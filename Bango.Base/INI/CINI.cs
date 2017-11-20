using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base.INI
{
    public class CINI
    {
        private string m_FileName;
        private bool m_readOnly;
        private Encoding _encoding = Encoding.ASCII;

        // The Win32 API methods
        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, string key, string value, string fileName);
        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, string key, [MarshalAs(UnmanagedType.LPArray)] byte[] result, string fileName);
        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, string key, int value, string fileName);
        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, int key, string value, string fileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string fileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, int key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int section, string key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);



        //public CINI()
        //{
        //    m_FileName = "";
        //}

        public CINI(string filename, bool readonlyfile)
            : this(filename, Encoding.ASCII, readonlyfile)
        {

        }

        public CINI(string filename, Encoding encoding, bool readonlyfile)
        {
            if (filename.Length < 1)
                throw new InvalidOperationException("Object creation not allowed because File Name property is null or empty.");

            m_FileName = filename;

            int i = filename.LastIndexOf(".ini", StringComparison.OrdinalIgnoreCase);
            if (!((i > 0) && (i == filename.Length - 4)))
                m_FileName += ".ini";

            _encoding = encoding;

            m_readOnly = readonlyfile;

        }

        /// <summary>
        ///   Gets or Set the default name for the INI file. </summary>
        /// <remarks>
        public string FileName
        {
            get
            {
                return m_FileName;
            }
            set
            {
                if (value.Length < 1)
                    throw new InvalidOperationException("Object creation not allowed because File Name property is null or empty.");

                m_FileName = value;
            }
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        protected internal void VerifyNotReadOnly()
        {
            if (m_readOnly)
                throw new InvalidOperationException("Operation not allowed because ReadOnly property is true.");
        }

        protected internal void VerifyName()
        {
            if (m_FileName == null || m_FileName == "")
                throw new InvalidOperationException("Operation not allowed because File Name property is null or empty.");
        }

        protected internal void VerifyAndAdjustSection(ref string section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            section = section.Trim();
        }

        protected internal void VerifyAndAdjustEntry(ref string entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            entry = entry.Trim();
        }

        protected virtual bool HasEntry(string section, string entry)
        {
            string[] entries = GetEntryNames(section);

            if (entries == null)
                return false;

            VerifyAndAdjustEntry(ref entry);
            return Array.IndexOf(entries, entry) >= 0;
        }

        protected virtual bool HasSection(string section)
        {
            string[] sections = GetSectionNames();

            if (sections == null)
                return false;

            VerifyAndAdjustSection(ref section);
            return Array.IndexOf(sections, section) >= 0;
        }


        /// <summary>
        ///   Sets the value for an entry inside a section. </summary>
        /// <param name="section">
        ///   The name of the section that holds the entry. </param>
        /// <param name="entry">
        ///   The name of the entry where the value will be set. </param>
        /// <param name="value">
        ///   The value to set. If it's null, the entry is removed. </param>
        /// <exception cref="InvalidOperationException">
        ///   <see cref="CINI" /> is true or
        ///   <see cref="Profile.Name" /> is null or empty. </exception>
        /// <exception cref="ArgumentNullException">
        ///   Either section or entry is null. </exception>
        /// <exception cref="Win32Exception">
        ///   The <see cref="WritePrivateProfileString" /> API failed. </exception>
        /// <seealso cref="GetValue" />
        public void SetValue(string section, string entry, object value)
        {
            // If the value is null, remove the entry
            //if (value == null)
            //{
            //    RemoveEntry(section, entry);
            //    return;
            //}

            VerifyNotReadOnly();
            VerifyName();
            VerifyAndAdjustSection(ref section);
            VerifyAndAdjustEntry(ref entry);

            //if (WritePrivateProfileString(section, entry, value.ToString(), m_FileName) == 0)
            //    throw new Win32Exception();

            byte[] arr = _encoding.GetBytes(value.ToString());
            if (WritePrivateProfileString(section, entry, arr, m_FileName) == 0)
                throw new Win32Exception();

            return;
        }

        /// <summary>
        ///   Retrieves the value of an entry inside a section. </summary>
        /// <param name="section">
        ///   The name of the section that holds the entry with the value. </param>
        /// <param name="entry">
        ///   The name of the entry where the value is stored. </param>
        /// <returns>
        ///   The return value is the entry's value, or null if the entry does not exist. </returns>
        /// <exception cref="InvalidOperationException">
        ///	  <see cref="m_FileName" /> is null or empty. </exception>
        /// <exception cref="ArgumentNullException">
        ///   Either section or entry is null. </exception>
        /// <seealso cref="SetValue" />
        /// <seealso cref="HasEntry" />
        public object GetValue(string section, string entry)
        {
            VerifyName();
            VerifyAndAdjustSection(ref section);
            VerifyAndAdjustEntry(ref entry);

            // Loop until the buffer has grown enough to fit the value
            for (int maxSize = 250; true; maxSize *= 2)
            {
                //StringBuilder result = new StringBuilder(maxSize);
                //int size = GetPrivateProfileString(section, entry, "", result, maxSize, m_FileName);
                byte[] bytes = new byte[maxSize];
                int size = GetPrivateProfileString(section, entry, "", bytes, maxSize, m_FileName);

                if (size < maxSize - 2)
                {
                    if (size == 0 && !HasEntry(section, entry))
                        return null;

                    string val = _encoding.GetString(bytes, 0, size);
                    return val;
                }
            }
        }

        /// <summary>
        ///   Retrieves the value of an entry inside a section. True if value is 1 else false in all case</summary>
        /// <param name="section">
        ///   The name of the section that holds the entry with the value. </param>
        /// <param name="entry">
        ///   The name of the entry where the value is stored. </param>
        /// <returns>
        ///   The return value is the entry's value, or null if the entry does not exist. </returns>
        /// <exception cref="InvalidOperationException">
        ///	  <see cref="m_FileName" /> is null or empty. </exception>
        /// <exception cref="ArgumentNullException">
        ///   Either section or entry is null. </exception>
        /// <seealso cref="SetValue" />
        /// <seealso cref="HasEntry" />
        public bool GetBoolValue(string section, string entry)
        {
            object obj;
            obj = GetValue(section, entry);
            if (obj != null)
            {
                if (obj.ToString().Trim().Equals("1"))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///   Removes an entry from a section. </summary>
        /// <param name="section">
        ///   The name of the section that holds the entry. </param>
        /// <param name="entry">
        ///   The name of the entry to remove. </param>
        /// <exception cref="InvalidOperationException">
        ///	  <see cref="m_FileName" /> is null or empty or
        ///   <see cref="Profile.ReadOnly" /> is true. </exception>
        /// <exception cref="ArgumentNullException">
        ///   Either section or entry is null. </exception>
        /// <exception cref="Win32Exception">
        ///   The <see cref="WritePrivateProfileString" /> API failed. </exception>
        /// <seealso cref="RemoveSection" />
        public void RemoveEntry(string section, string entry)
        {
            // Verify the entry exists
            if (!HasEntry(section, entry))
                return;

            VerifyNotReadOnly();
            VerifyName();
            VerifyAndAdjustSection(ref section);
            VerifyAndAdjustEntry(ref entry);

            if (WritePrivateProfileString(section, entry, 0, m_FileName) == 0)
                throw new Win32Exception();

        }


        /// <summary>
        ///   Removes a section. </summary>
        /// <param name="section">
        ///   The name of the section to remove. </param>
        /// <exception cref="InvalidOperationException">
        ///	  <see cref="m_FileName" /> is null or empty or
        ///   <see cref="CINI" /> is true. </exception>
        /// <exception cref="ArgumentNullException">
        ///   section is null. </exception>
        /// <exception cref="Win32Exception">
        ///   The <see cref="WritePrivateProfileString" /> API failed. </exception>
        /// <seealso cref="RemoveEntry" />
        public void RemoveSection(string section)
        {
            // Verify the section exists
            if (!HasSection(section))
                return;

            VerifyNotReadOnly();
            VerifyName();
            VerifyAndAdjustSection(ref section);


            if (WritePrivateProfileString(section, 0, "", m_FileName) == 0)
                throw new Win32Exception();

        }

        /// <summary>
        ///   Retrieves the names of all the entries inside a section. </summary>
        /// <param name="section">
        ///   The name of the section holding the entries. </param>
        /// <returns>
        ///   If the section exists, the return value is an array with the names of its entries; 
        ///   otherwise it's null. </returns>
        /// <exception cref="InvalidOperationException">
        ///	  <see cref="m_FileName" /> is null or empty. </exception>
        /// <seealso cref="HasEntry" />
        /// <seealso cref="GetSectionNames" />
        public string[] GetEntryNames(string section)
        {
            // Verify the section exists
            if (!HasSection(section))
                return null;

            VerifyAndAdjustSection(ref section);

            // Loop until the buffer has grown enough to fit the value
            for (int maxSize = 500; true; maxSize *= 2)
            {
                byte[] bytes = new byte[maxSize];
                int size = GetPrivateProfileString(section, 0, "", bytes, maxSize, m_FileName);

                if (size < maxSize - 2)
                {
                    // Convert the buffer to a string and split it
                    string entries = _encoding.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    if (entries == "")
                        return new string[0];
                    return entries.Split(new char[] { '\0' });
                }
            }
        }

        /// <summary>
        ///   Retrieves the names of all the sections. </summary>
        /// <returns>
        ///   If the INI file exists, the return value is an array with the names of all the sections;
        ///   otherwise it's null. </returns>
        /// <seealso cref="HasSection" />
        /// <seealso cref="GetEntryNames" />
        public string[] GetSectionNames()
        {
            // Verify the file exists
            if (!File.Exists(m_FileName))
                return null;

            // Loop until the buffer has grown enough to fit the value
            for (int maxSize = 500; true; maxSize *= 2)
            {
                byte[] bytes = new byte[maxSize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxSize, m_FileName);

                if (size < maxSize - 2)
                {
                    // Convert the buffer to a string and split it
                    string sections = _encoding.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    if (sections == "")
                        return new string[0];
                    return sections.Split(new char[] { '\0' });
                }
            }
        }



    }
}
