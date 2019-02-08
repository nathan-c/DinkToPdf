﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DinkToPdf.Contracts;
using DinkToPdf.EventDefinitions;

namespace DinkToPdf
{
    public class BasicConverter : IConverter
    {
        public readonly ITools Tools;
        private readonly VoidCallback _onPhaseChangedDelegate;
        private readonly VoidCallback _onProgressChangedDelegate;
        private readonly IntCallback _onFinishedDelegate;
        private readonly StringCallback _onWarningDelegate;
        private readonly StringCallback _onErrorDelegate;

        public BasicConverter(ITools tools)
        {
            Tools = tools;

            // This is needed to make sure callbacks are not garbage collected
            _onPhaseChangedDelegate = new VoidCallback(OnPhaseChanged);
            _onProgressChangedDelegate = new VoidCallback(OnProgressChanged);
            _onFinishedDelegate = new IntCallback(OnFinished);
            _onWarningDelegate = new StringCallback(OnWarning);
            _onErrorDelegate = new StringCallback(OnError);
        }

        public IDocument ProcessingDocument { get; private set; }

        public event EventHandler<PhaseChangedArgs> PhaseChanged;

        public event EventHandler<ProgressChangedArgs> ProgressChanged;

        public event EventHandler<FinishedArgs> Finished;

        public event EventHandler<ErrorArgs> Error;

        public event EventHandler<WarningArgs> Warning;

        public virtual byte[] Convert(IDocument document)
        {
            try
            {
                if (!document.GetObjects().Any())
                {
                    throw new ArgumentException(
                        "No objects is defined in document that was passed. At least one object must be defined.");
                }

                ProcessingDocument = document;

                var result = new byte[0];
                Tools.Load();

                var converter = CreateConverter(document);

                //register events
                Tools.SetPhaseChangedCallback(converter, _onPhaseChangedDelegate);
                Tools.SetProgressChangedCallback(converter, _onProgressChangedDelegate);
                Tools.SetFinishedCallback(converter, _onFinishedDelegate);
                Tools.SetWarningCallback(converter, _onWarningDelegate);
                Tools.SetErrorCallback(converter, _onErrorDelegate);

                var converted = Tools.DoConversion(converter);

                if (converted)
                {
                    result = Tools.GetConversionResult(converter);
                }

                Tools.DestroyConverter(converter);
                
                return result;
            }
            finally
            {
                ProcessingDocument = null;
            }
        }

        private void OnPhaseChanged(IntPtr converter)
        {
            var currentPhase = Tools.GetCurrentPhase(converter);
            var eventArgs = new PhaseChangedArgs
            {
                Document = ProcessingDocument,
                PhaseCount = Tools.GetPhaseCount(converter),
                CurrentPhase = currentPhase,
                Description = Tools.GetPhaseDescription(converter, currentPhase)
            };

            PhaseChanged?.Invoke(this, eventArgs);
        }

        private void OnProgressChanged(IntPtr converter)
        {
            var eventArgs = new ProgressChangedArgs
            {
                Document = ProcessingDocument,
                Description = Tools.GetProgressString(converter)
            };

            ProgressChanged?.Invoke(this, eventArgs);
        }

        private void OnFinished(IntPtr converter, int success)
        {
            var eventArgs = new FinishedArgs
            {
                Document = ProcessingDocument,
                Success = success == 1 ? true : false
            };

            Finished?.Invoke(this, eventArgs);
        }

        private void OnError(IntPtr converter, string message)
        {
            var eventArgs = new ErrorArgs
            {
                Document = ProcessingDocument,
                Message = message
            };

            Error?.Invoke(this, eventArgs);
        }

        private void OnWarning(IntPtr converter, string message)
        {
            var eventArgs = new WarningArgs
            {
                Document = ProcessingDocument,
                Message = message
            };

            Warning?.Invoke(this, eventArgs);
        }

        private IntPtr CreateConverter(IDocument document)
        {
            var converter = IntPtr.Zero;

            {
                var settings = Tools.CreateGlobalSettings();

                ApplyConfig(settings, document, true);

                converter = Tools.CreateConverter(settings);
            }

            foreach (var obj in document.GetObjects())
            {
                if (obj != null)
                {
                    var settings = Tools.CreateObjectSettings();

                    ApplyConfig(settings, obj, false);

                    Tools.AddObject(converter, settings, obj.GetContent());
                }
            }

            return converter;
        }

        private void ApplyConfig(IntPtr config, ISettings settings, bool isGlobal)
        {
            if (settings == null)
            {
                return;
            }

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var props = settings.GetType().GetProperties(bindingFlags);

            foreach (var prop in props)
            {
                var attrs = (Attribute[]) prop.GetCustomAttributes();
                var propValue = prop.GetValue(settings);

                if (propValue == null)
                {
                }
                else if (attrs.Length > 0 && attrs[0] is WkHtmlAttribute)
                {
                    var attr = attrs[0] as WkHtmlAttribute;

                    Apply(config, attr.Name, propValue, isGlobal);
                }
                else if (propValue is ISettings)
                {
                    ApplyConfig(config, propValue as ISettings, isGlobal);
                }
            }
        }

        private void Apply(IntPtr config, string name, object value, bool isGlobal)
        {
            var type = value.GetType();

            Func<IntPtr, string, string, int> applySetting;
            if (isGlobal)
            {
                applySetting = Tools.SetGlobalSetting;
            }
            else
            {
                applySetting = Tools.SetObjectSetting;
            }

            if (typeof(bool) == type)
            {
                applySetting(config, name, (bool) value ? "true" : "false");
            }
            else if (typeof(double) == type)
            {
                applySetting(config, name, ((double) value).ToString("0.##", CultureInfo.InvariantCulture));
            }
            else if (typeof(Dictionary<string, string>).IsAssignableFrom(type))
            {
                var dictionary = (Dictionary<string, string>) value;
                var index = 0;

                foreach (var pair in dictionary)
                {
                    if (pair.Key == null || pair.Value == null)
                    {
                        continue;
                    }

                    //https://github.com/wkhtmltopdf/wkhtmltopdf/blob/c754e38b074a75a51327df36c4a53f8962020510/src/lib/reflect.hh#L192
                    applySetting(config, name + ".append", null);
                    applySetting(config, string.Format("{0}[{1}]", name, index), pair.Key + "\n" + pair.Value);

                    index++;
                }
            }
            else
            {
                applySetting(config, name, value.ToString());
            }
        }
    }
}