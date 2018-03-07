using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Engine
{
    public class ReflectionUtils
    {
        public static Type[] GetTypesByName(string className, bool ignoreSystemTypes)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => {
                if (type.Name == className)
                {
                    if (ignoreSystemTypes)
                    {
                        if (!type.Namespace.StartsWith("System."))
                            return true;
                        else
                            return false;
                    }
                    return true;
                }
                return false;
            }).ToArray();
        }
    }

    public class MarshalHelper
    {
        public static IntPtr StringToPtrAnsi(string str)
        {
            if (string.IsNullOrEmpty(str))
                return IntPtr.Zero;

            byte[] bytes = Encoding.ASCII.GetBytes(str + '\0');
            IntPtr result = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, result, bytes.Length);

            return result;
        }
    }

    // TODO: Get rid of this
    class CString
    {
        public static unsafe char* Strstr(char* haystack, char* needle)
        {
            if (haystack == null || needle == null)
                return null;

            for (; *haystack != 0; haystack++)
            {
                char* h, n;
                for (h = haystack, n = needle; *h != 0 && *n != 0 && (*h == *n); ++h, ++n) ;
                if (*n == '\0')
                    return haystack;
            }
            return null;
        }

        public static unsafe string GetBlock(char* str, char** outPosition = null)
        {
            fixed (char* brace = "}")
            {
                char* end = Strstr(str, brace);
                if (end == null)
                    return new string(str);

                if (outPosition != null)
                    *outPosition = end;
                long length = end - str + 1;
                return new string(str, 0, (int)length);
            }
        }

        public static unsafe string GetBlock(string str, int offset)
        {
            fixed (char* strPtr = str + offset)
                return GetBlock(strPtr);
        }

        public static unsafe string GetStatement(char* str, char** outPosition)
        {
            fixed (char* semiColon = ";")
            {
                char* end = Strstr(str, semiColon);
                if (end == null)
                    return new string(str);

                if (outPosition != null)
                    *outPosition = end;
                long length = end - str + 1;
                return new string(str, 0, (int)length);
            }
        }

        public static unsafe char* FindToken(char* str, string token)
        {
            char* t = str;
            fixed (char* tokenPtr = token)
            {
                while ((t = Strstr(t, tokenPtr)) != null)
                {
                    bool left = str == t || char.IsWhiteSpace(t[-1]);
                    char? c = t[token.Length];
                    bool right = c != null || char.IsWhiteSpace(t[token.Length]);
                    if (left && right)
                        return t;

                    t += token.Length;
                }
                return null;
            }
        }

        public static unsafe int FindStringPosition(string str, string search, int offset = 0)
        {
            fixed (char* strPtr = str + offset, searchPtr = search)
            {
                char* found = Strstr(strPtr, searchPtr);
                if (found == null)
                    return -1;
                return (int)(found - strPtr) + offset;
            }
        }
    }
}
