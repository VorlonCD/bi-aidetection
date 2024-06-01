using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;

using Microsoft.Win32.SafeHandles;

namespace AITool
{
    public static class StringExtensions
    {
        public static bool Within(this string value, string findStr, int maxlen = 0, string trimStartChars = " .>-*")
        {
            if (value.IsEmpty() || findStr.IsEmpty())
                return false;

            if (maxlen > 0 && value.Length > maxlen)
                value = value.Substring(0, maxlen);

            //trim the start of the string to get rid of any leading spaces or periods
            value = value.TrimStart(trimStartChars.ToCharArray());

            return value.Contains(findStr, StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsNumeric(this string theValue)
        {
            double retNum;
            return double.TryParse(theValue.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

        [DebuggerStepThrough]
        public static string JoinStr(this List<string> values, string separator)
        {
            //this custom join should not be used for CSV type output because it does not include empty items
            if (values == null)
                throw new ArgumentNullException("values");

            if (values.Count == 0)
                return String.Empty;

            if (separator == null)
                separator = String.Empty;

            StringBuilder sb = new StringBuilder("");

            for (int i = 0; i < values.Count; i++)
            {
                if (!values[i].IsEmpty())
                    sb.Append(values[i].Trim() + separator);
            }

            return sb.ToString().Trim(separator.ToCharArray());

        }

        [DebuggerStepThrough]
        public static List<string> SplitStr(this string InList, string Separators, bool RemoveEmpty = true, bool TrimStr = true, bool ToLower = false, string TrimChars = " ")
        {
            List<string> Ret = new List<string>();
            if (!string.IsNullOrWhiteSpace(InList))
            {
                StringSplitOptions SSO = StringSplitOptions.None;

                if (RemoveEmpty)
                    SSO = StringSplitOptions.RemoveEmptyEntries;

                string[] splt = InList.Split(Separators.ToCharArray(), SSO);
                for (int i = 0; i < splt.Length; i++)
                {
                    if (ToLower)
                        splt[i] = splt[i].ToLower();

                    if (RemoveEmpty && !string.IsNullOrWhiteSpace(splt[i]))
                    {
                        if (TrimStr)
                            Ret.Add(splt[i].Trim(TrimChars));
                        else
                            Ret.Add(splt[i]);
                    }
                    else if (!RemoveEmpty)
                    {
                        if (TrimStr)
                            Ret.Add(splt[i].Trim());
                        else
                            Ret.Add(splt[i]);
                    }

                }
            }
            return Ret;
        }

        //create a string extension that prepends a string to a string if it doesnt already have it.  If it does already have it then move it to the front.  The extension should have a parameter for the separator character.  The extension should be case insensitive 
        //example:  "1,2,3,4,5"  prepend "3" with separator ","  result:  "3,1,2,4,5"



        //public static string Prepend(this string input, string valueToPrepend, int maxItems = 8, char separator = ',')
        //{
        //    string val = valueToPrepend.Trim(separator, ' ');

        //    if (string.IsNullOrWhiteSpace(val))
        //        return input;

        //    var resultBuilder = new StringBuilder();
        //    var uniqueValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //    uniqueValues.Add(val);
        //    resultBuilder.Append(val);

        //    int itemCount = 1; // Start from 1 as we have already added the valueToPrepend

        //    foreach (var item in input.Split(separator).Select(s => s.Trim()))
        //    {
        //        if (!string.IsNullOrEmpty(item) && !uniqueValues.Contains(item) && itemCount < maxItems)
        //        {
        //            resultBuilder.Append(separator);
        //            resultBuilder.Append(item);
        //            uniqueValues.Add(item);
        //            itemCount++;
        //        }
        //    }

        //    return resultBuilder.ToString();
        //}

        public static string Prepend(this string input, string valueToPrepend, int maxItems = 6, char separator = ',')
        {
            string val = valueToPrepend.Trim(separator, ' ');
            var inputValues = input.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

            // Using StringBuilder for efficient string concatenation
            var resultBuilder = new StringBuilder(val);

            int itemCount = 1; // Start from 1 as we have already added the valueToPrepend
            foreach (var item in inputValues)
            {
                if (!StringComparer.OrdinalIgnoreCase.Equals(item, val) && itemCount < maxItems)
                {
                    resultBuilder.Append(separator);
                    resultBuilder.Append(item);
                    itemCount++;
                }
            }

            return resultBuilder.ToString();
        }
        //[DebuggerStepThrough]
        public static string Append(this string value, string newvalue, string Separators, string ListSeparators = ",;")
        {
            //appends only if the string doesnt already have it and trims the separator characters 

            if (newvalue.IsEmpty())
                return value;

            if (value.IsEmpty())
                return newvalue.Trim((Separators + ListSeparators).ToCharArray());

            List<string> newlist = newvalue.SplitStr(ListSeparators);
            List<string> existinglist = value.SplitStr((Separators + ListSeparators).Replace(" ", ""));
            string newstr = "";
            foreach (var item in newlist)
            {
                if (!Global.IsInList(item, existinglist, TrueIfEmpty: true))
                {
                    newstr += item + ", ";
                }
            }

            return (value.Trim((Separators + ListSeparators).ToCharArray()) + Separators + newstr).Trim((Separators + ListSeparators).ToCharArray());

        }
        [DebuggerStepThrough]
        public static string Truncate(this string value, int maxLength = 512, bool ellipsis = true)
        {
            if (string.IsNullOrEmpty(value)) return value;

            if (value.Length <= maxLength) return value;

            if (ellipsis) return value.Substring(0, maxLength) + "...";

            return value.Substring(0, maxLength);

        }
        [DebuggerStepThrough]
        public static string Trim(this string value, string trimstrlist)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return value.Trim(trimstrlist.ToCharArray());

        }
        [DebuggerStepThrough]
        public static string ReplaceChars(this string value, char replacechar)
        {
            if (string.IsNullOrEmpty(value)) return value;

            return new string(replacechar, value.Length);

        }

        [DebuggerStepThrough]
        public static double ToDouble(this string value)
        {
            double outdbl = 0;

            //Take into account that some countries may use 123,45 vs 123.45
            if (!value.IsNull() && double.TryParse(value.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out outdbl))
                return outdbl;
            else
                return 0;
        }
        public static float ToFloat(this string value)
        {
            float outdbl = 0;

            //Take into account that some countries may use 123,45 vs 123.45
            if (!value.IsNull() && float.TryParse(value.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out outdbl))
                return outdbl;
            else
                return 0;
        }

        [DebuggerStepThrough]
        public static int ToInt(this string value)
        {
            if (!value.IsNull())
                return Convert.ToInt32(value.Trim());
            else
                return 0;
        }

        [DebuggerStepThrough]
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsNull(this object obj)
        {
            if (obj == null)
                return true;

            if (obj is string && string.IsNullOrWhiteSpace((string)obj))
                return true;

            if (obj is IntPtr && (IntPtr)obj == IntPtr.Zero)
                return true;

            if (obj is UIntPtr && (UIntPtr)obj == UIntPtr.Zero)
                return true;

            if (obj is SafeFileHandle && ((SafeFileHandle)obj).IsInvalid)
                return true;

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsNotNull(this object obj)
        {
            return !obj.IsNull();
        }
        [DebuggerStepThrough]
        public static string CleanString(this string inp, string ReplaceStr = " ")
        {
            if (inp == null || string.IsNullOrWhiteSpace(inp))
            {
                return "";
            }
            else
            {
                return inp.Replace("\0", ReplaceStr).Replace("\r", ReplaceStr).Replace("\n", ReplaceStr);
            }
        }
        [DebuggerStepThrough]
        public static string UpperFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            //char[] a = s.ToCharArray();
            //a[0] = char.ToUpper(a[0]);
            //return new string(a);
            return s.FormatPascalAndAcronym();
        }
        private static string FormatPascalAndAcronym(this string input)
        {
            //Input:  "QWERTYSomeThing OmitTRYSomeThing MayBeWorkingFYI"
            //Output: "QWERTY Some Thing Omit TRY Some Thing May Be Working FYI" 

            var builder = new StringBuilder(Char.ToUpper(input[0]).ToString());
            if (builder.Length > 0)
            {
                for (var index = 1; index < input.Length; index++)
                {
                    char prevChar = input[index - 1];
                    char nextChar = index + 1 < input.Length ? input[index + 1] : '\0';

                    bool isNextLower = Char.IsLower(nextChar);
                    bool isNextUpper = Char.IsUpper(nextChar);
                    bool isPresentUpper = Char.IsUpper(input[index]);
                    bool isPrevLower = Char.IsLower(prevChar);
                    bool isPrevUpper = Char.IsUpper(prevChar);
                    bool PrevSpace = Char.IsWhiteSpace(prevChar);

                    if (!string.IsNullOrWhiteSpace(prevChar.ToString()) &&
                        ((isPrevUpper && isPresentUpper && isNextLower) ||
                        (isPrevLower && isPresentUpper && isNextLower) ||
                        (isPrevLower && isPresentUpper && isNextUpper)))
                    {
                        builder.Append(' ');
                        builder.Append(Char.ToUpper(input[index]));
                    }
                    else if (PrevSpace)
                        builder.Append(Char.ToUpper(input[index]));

                    else
                    {
                        builder.Append(input[index]);
                    }
                }
            }
            return builder.ToString();
        }
        public static bool IsStringBefore(this string teststring, string first, string second)
        {

            //test something like this - make sure we arnt picking up the semicolon that could be part of a URL:
            //person, car ; http://URL/;
            bool ret = false;
            int firstidx = teststring.IndexOf(first, StringComparison.OrdinalIgnoreCase);

            if (firstidx > -1)
            {
                int secondidx = teststring.IndexOf(second, StringComparison.OrdinalIgnoreCase);
                if (secondidx > -1)
                {
                    if (firstidx < secondidx)
                    {
                        ret = true;
                    }
                }
                else
                {
                    ret = true;
                }
            }

            return ret;

        }
        [DebuggerStepThrough]
        public static bool Has(this string value, string FindStr)
        {
            return !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(FindStr)
                   && value.Contains(FindStr, StringComparison.OrdinalIgnoreCase);

        }
        [DebuggerStepThrough]
        public static bool EqualsIgnoreCase(this string value, string FindStr)
        {
            if (string.Equals(value, FindStr, StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;
        }
        [DebuggerStepThrough]
        public static string GetWord(this string InpStr, string JustBefore, string JustAfter, Int32 LastPos = 0, Int32 FirstPos = 0, bool NoTrim = false, bool MustFindJustAfter = false)
        {
            string Ret = "";

            try
            {
                string[] JB = JustBefore.Split('|');
                string[] JA = JustAfter.Split('|');
                int JBPos = 0;
                int JAPos = 0;
                string BefStr = "";
                string AftStr = "";
                int WordLen = 0;
                string RetWord = "";

                if (JustBefore.Length > 0)
                {
                    foreach (string BefStrTmp in JB)
                    {
                        BefStr = BefStrTmp;
                        if (BefStr.Length > 0)
                        {
                            JBPos = InpStr.IndexOf(BefStr, FirstPos, StringComparison.OrdinalIgnoreCase);
                            if (JBPos >= 0)
                                break;
                        }
                    }
                }
                else
                    JBPos = FirstPos;
                if (JBPos == -1)
                    return Ret;
                int FirstFnd = InpStr.Length;
                foreach (string AftStrTmp in JA)
                {
                    AftStr = AftStrTmp;
                    if (AftStr.Length > 0)
                    {
                        Int32 count = InpStr.Length - (JBPos + BefStr.Length);
                        Int32 StartIndex = JBPos + BefStr.Length;
                        JAPos = InpStr.IndexOf(AftStr, StartIndex, count, StringComparison.OrdinalIgnoreCase);
                        if (JAPos >= 0)
                            // If JAPos <= FirstFnd Then FirstFnd = JAPos
                            FirstFnd = Math.Min(JAPos, FirstFnd);
                    }
                }

                // If FirstFnd <= JAPos Then
                JAPos = FirstFnd;
                // End If

                if (JAPos == -1 || JAPos == 0 || JustAfter.Length == 0)
                {
                    if (!MustFindJustAfter)
                        JAPos = InpStr.Length;
                }

                if (JAPos <= JBPos)
                    return Ret;

                WordLen = JAPos - (JBPos + BefStr.Length);
                if (WordLen > 0)
                {
                    RetWord = InpStr.Substring(JBPos + BefStr.Length, WordLen);
                    LastPos = JAPos; // JBPos + BefStr.Length + RetWord.Length
                    if (NoTrim)
                        Ret = RetWord;
                    else
                        Ret = RetWord.Trim();
                }
            }
            // Return ""

            catch (Exception)
            {
            }
            finally
            {

            }

            return Ret;
        }

        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("sdsgtj;lrjwteojtkslkdjsl;dvlbmv.bmvlfu7r0tret-rereigjejgkgljg42");

        //this is not truly secure, but better than storing plain text in the JSON file
        public static string Encrypt(this string input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(input),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(this string encryptedData)
        {
            if (String.IsNullOrEmpty(encryptedData))
                return "";

            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            catch
            {
                return "";
            }
        }

        public static SecureString ToSecureString(this string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(this SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }

        /// <summary>
        /// Implement's VB's Like operator logic.
        /// </summary>
        public static bool IsLike(this string s, string pattern)
        {
            // Characters matched so far
            int matched = 0;

            // Loop through pattern string
            for (int i = 0; i < pattern.Length;)
            {
                // Check for end of string
                if (matched > s.Length)
                    return false;

                // Get next pattern character
                char c = pattern[i++];
                if (c == '[') // Character list
                {
                    // Test for exclude character
                    bool exclude = (i < pattern.Length && pattern[i] == '!');
                    if (exclude)
                        i++;
                    // Build character list
                    int j = pattern.IndexOf(']', i);
                    if (j < 0)
                        j = s.Length;
                    HashSet<char> charList = CharListToSet(pattern.Substring(i, j - i));
                    i = j + 1;

                    if (charList.Contains(s[matched]) == exclude)
                        return false;
                    matched++;
                }
                else if (c == '?') // Any single character
                {
                    matched++;
                }
                else if (c == '#') // Any single digit
                {
                    if (!Char.IsDigit(s[matched]))
                        return false;
                    matched++;
                }
                else if (c == '*') // Zero or more characters
                {
                    if (i < pattern.Length)
                    {
                        // Matches all characters until
                        // next character in pattern
                        char next = pattern[i];
                        int j = s.IndexOf(next, matched);
                        if (j < 0)
                            return false;
                        matched = j;
                    }
                    else
                    {
                        // Matches all remaining characters
                        matched = s.Length;
                        break;
                    }
                }
                else // Exact character
                {
                    if (matched >= s.Length || c != s[matched])
                        return false;
                    matched++;
                }
            }
            // Return true if all characters matched
            return (matched == s.Length);
        }

        /// <summary>
        /// Converts a string of characters to a HashSet of characters. If the string
        /// contains character ranges, such as A-Z, all characters in the range are
        /// also added to the returned set of characters.
        /// </summary>
        /// <param name="charList">Character list string</param>
        private static HashSet<char> CharListToSet(string charList)
        {
            HashSet<char> set = new HashSet<char>();

            for (int i = 0; i < charList.Length; i++)
            {
                if ((i + 1) < charList.Length && charList[i + 1] == '-')
                {
                    // Character range
                    char startChar = charList[i++];
                    i++; // Hyphen
                    char endChar = (char)0;
                    if (i < charList.Length)
                        endChar = charList[i++];
                    for (int j = startChar; j <= endChar; j++)
                        set.Add((char)j);
                }
                else set.Add(charList[i]);
            }
            return set;
        }

    }
}
