using System;
using System.Runtime.InteropServices;

namespace DinkToPdf
{
    public static unsafe class WkHtmlToXBindings
    {
        private const string DLLNAME = "libwkhtmltox";

        private const CharSet CHARSET = CharSet.Unicode;

#if NETFRAMEWORK
        [DllImport("kernel32",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Auto,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true,
            SetLastError = true)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        public static string GetLibraryPathname(string filename)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            // If 64-bit process, load 64-bit DLL
            var prefix = Environment.Is64BitProcess ? "x64" : "x86";
            if (assembly?.Location != null)
            {
                var dir = System.IO.Path.GetDirectoryName(assembly.Location);
                prefix = System.IO.Path.Combine(dir, prefix);
            }
            return System.IO.Path.Combine(prefix, filename);
        }

        static WkHtmlToXBindings()
        {
            // Get 32-bit/64-bit library directory
            var libPath = GetLibraryPathname(DLLNAME + ".dll");
#if DEBUG
            System.Diagnostics.Trace.WriteLine($"About to load {libPath}");
#endif
            var dllhandle = LoadLibrary(libPath);
            // Handle error loading
            if (dllhandle == IntPtr.Zero)
            {
                System.Diagnostics.Trace.WriteLine($"Failed loading {libPath}");
                return;
            }
        }
#endif

        #region HTML to PDF bindings

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_extended_qt();

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_version();

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_init(int useGraphics);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_deinit();

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_global_settings();

        [DllImport(DLLNAME, CharSet = CHARSET)]
        public static extern int wkhtmltopdf_set_global_setting(IntPtr settings,
            [MarshalAs((short) CustomUnmanagedType.LPUTF8Str)]
            string name,
            [MarshalAs((short) CustomUnmanagedType.LPUTF8Str)]
            string value);


        [DllImport(DLLNAME, CharSet = CHARSET)]
        public static extern int wkhtmltopdf_get_global_setting(IntPtr settings,
            [MarshalAs((short) CustomUnmanagedType.LPUTF8Str)]
            string name,
            byte* value, int valueSize);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_destroy_global_settings(IntPtr settings);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_object_settings();

        [DllImport(DLLNAME, CharSet = CHARSET)]
        public static extern int wkhtmltopdf_set_object_setting(IntPtr settings,
            [MarshalAs((short) CustomUnmanagedType.LPUTF8Str)]
            string name,
            [MarshalAs((short) CustomUnmanagedType.LPUTF8Str)]
            string value);

        [DllImport(DLLNAME, CharSet = CHARSET)]
        public static extern int wkhtmltopdf_get_object_setting(IntPtr settings,
            [MarshalAs((short) CustomUnmanagedType.LPUTF8Str)]
            string name,
            byte* value, int valueSize);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_destroy_object_settings(IntPtr settings);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_create_converter(IntPtr globalSettings);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_add_object(IntPtr converter,
            IntPtr objectSettings,
            byte[] data);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_add_object(IntPtr converter,
            IntPtr objectSettings,
            [MarshalAs((short) CustomUnmanagedType.LPUTF8Str)]
            string data);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool wkhtmltopdf_convert(IntPtr converter);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern void wkhtmltopdf_destroy_converter(IntPtr converter);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_get_output(IntPtr converter, out IntPtr data);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_phase_changed_callback(IntPtr converter,
            [MarshalAs(UnmanagedType.FunctionPtr)] VoidCallback callback);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_progress_changed_callback(IntPtr converter,
            [MarshalAs(UnmanagedType.FunctionPtr)] VoidCallback callback);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_finished_callback(IntPtr converter,
            [MarshalAs(UnmanagedType.FunctionPtr)] IntCallback callback);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_warning_callback(IntPtr converter,
            [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback callback);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_set_error_callback(IntPtr converter,
            [MarshalAs(UnmanagedType.FunctionPtr)] StringCallback callback);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_phase_count(IntPtr converter);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_current_phase(IntPtr converter);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_phase_description(IntPtr converter, int phase);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkhtmltopdf_progress_string(IntPtr converter);

        [DllImport(DLLNAME, CharSet = CHARSET, CallingConvention = CallingConvention.Cdecl)]
        public static extern int wkhtmltopdf_http_error_code(IntPtr converter);

        #endregion

        #region Image to  PDF bindings

        #endregion
    }
}