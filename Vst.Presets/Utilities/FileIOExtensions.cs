using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Vst.Presets.Utilities
{
    /// <summary>
    /// Describes an object that is capable of saving it's contents.
    /// </summary>
    public interface ISave
    {
        void Save(Stream stream);
    }

    public delegate T Loader<T>(Stream source);
    public delegate void Saver(Stream source);

    public static class FileIOExtensions
    {
        /// <summary>
        /// Saves the contents to the given path
        /// </summary>
        /// <param name="save">The object to be saved</param>
        /// <param name="path">The fully qualified path to save to contents to</param>
        public static void Save(this Saver save, FileInfo path)
        {
            path.EnsureDirectoryExists();
            using (var file = new FileStream(path.FullName, FileMode.Create, FileAccess.Write))
                save(file);
        }

        /// <summary>
        /// Saves the contents to the given path
        /// </summary>
        /// <param name="save">The object to be saved</param>
        /// <param name="path">The fully qualified path to save to contents to</param>
        public static void Save(this Saver save, string path)
        {
            Save(save, new FileInfo(path));
        }

        /// <summary>
        /// Writes the contents to a byte array
        /// </summary>
        /// <param name="save">The object to be saved</param>
        /// <param name="data">The resulting bytes</param>
        public static void Save(this Saver save, out byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                save(ms);
                data = ms.ToArray();
            }
        }

        /// <summary>
        /// Saves the contents to the given path
        /// </summary>
        /// <param name="owner">The object to be saved</param>
        /// <param name="path">The fully qualified path to save to contents to</param>
        public static void Save(this ISave owner, FileInfo path)
        {
            path.EnsureDirectoryExists();
            using (var file = new FileStream(path.FullName, FileMode.Create, FileAccess.Write))
                owner.Save(file);
        }

        /// <summary>
        /// Saves the contents to the given path
        /// </summary>
        /// <param name="owner">The object to be saved</param>
        /// <param name="path">The fully qualified path to save to contents to</param>
        public static void Save(this ISave owner, string path)
        {
            owner.Save(new FileInfo(path));
        }

        /// <summary>
        /// Writes the contents to a byte array
        /// </summary>
        /// <param name="owner">The object to be saved</param>
        /// <param name="data">The resulting bytes</param>
        public static void Save(this ISave owner, out byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                owner.Save(ms);
                data = ms.ToArray();
            }
        }

        [Obsolete("Use Save(out byte[] data)")]
        public static byte[] ToBytes(ISave owner)
        {
            Save(owner, out var result);
            return result;
        }

        ///// <summary>
        ///// Loads the contents from the given path
        ///// </summary>
        ///// <param name="loadable">The object to be loaded</param>
        ///// <param name="path">The fully qualified path to load from</param>
        //public static bool From<T>(this LoadFunction<T> loadable, Stream stream, out T target)
        //{
        //    return loadable(stream, out target);
        //}


        ///// <summary>
        ///// Loads the contents from the given path
        ///// </summary>
        ///// <param name="loadable">The object to be loaded</param>
        ///// <param name="path">The fully qualified path to load from</param>
        //public static bool From<T>(this LoadFunction<T> loadable, FileInfo path, out T target)
        //{
        //    using (var file = File.OpenRead(path.FullName))
        //        return loadable(file, out target);
        //}

        ///// <summary>
        ///// Loads the contents from the given path
        ///// </summary>
        ///// <param name="loadable">The object to be loaded</param>
        ///// <param name="path">The fully qualified path to load from</param>
        //public static bool From<T>(this LoadFunction<T> loadable, string path, out T target)
        //{
        //    return From(loadable, new FileInfo(path), out target);
        //}

        ///// <summary>
        ///// Loads the contents from the byte array
        ///// </summary>
        ///// <param name="loadable">The object to be loaded</param>
        ///// <param name="data">The bytes to read</param>
        //public static bool From<T>(this LoadFunction<T> loadable, byte[] data, out T target)
        //{
        //    using (var ms = new MemoryStream(data))
        //        return loadable(ms, out target);
        //}

        //// Needs to move out to PowerShell project
        //public static bool TryFrom<T>(this LoadFunction<T> loadable, object anything, out T target) //where T : class
        //{
        //    //if (anything is PSObject)
        //    //    anything = ((PSObject)anything).BaseObject;

        //    if (anything is T)
        //    {
        //        target = (T)anything;
        //        return true;
        //    }

        //    if (anything is string)
        //        return From(loadable, (string)anything, out target);

        //    if (anything is FileInfo)
        //        return From(loadable, ((FileInfo)anything).FullName, out target);

        //    if (anything is Stream)
        //        return loadable((Stream)anything, out target);

        //    if (anything is byte[])
        //        return From(loadable, (byte[])anything, out target);

        //    target = default(T);
        //    return false;
        //}



        /// <summary>
        /// Loads the contents from the given path
        /// </summary>
        /// <param name="loadable">The object to be loaded</param>
        /// <param name="path">The fully qualified path to load from</param>
        public static T From<T>(this Loader<T> loadable, Stream stream)
        {
            return loadable(stream);
        }

        /// <summary>
        /// Loads the contents from the given path
        /// </summary>
        /// <param name="loadable">The object to be loaded</param>
        /// <param name="path">The fully qualified path to load from</param>
        public static T From<T>(this Loader<T> loadable, FileInfo path)
        {
            using (var file = File.OpenRead(path.FullName))
                return loadable(file);
        }

        /// <summary>
        /// Loads the contents from the given path
        /// </summary>
        /// <param name="loadable">The object to be loaded</param>
        /// <param name="path">The fully qualified path to load from</param>
        public static T From<T>(this Loader<T> loadable, string path)
        {
            return From(loadable, new FileInfo(path));
        }

        /// <summary>
        /// Loads the contents from the byte array
        /// </summary>
        /// <param name="loadable">The object to be loaded</param>
        /// <param name="data">The bytes to read</param>
        public static T From<T>(this Loader<T> loadable, byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return loadable(ms);
        }

        // Needs to move out to PowerShell project
        public static T TryFrom<T>(this Loader<T> loadable, object anything) //where T : class
        {
            //if (anything is PSObject)
            //    anything = ((PSObject)anything).BaseObject;

            if (anything is T)
                return (T)anything;

            if (anything is string)
                return From(loadable, (string)anything);

            if (anything is FileInfo)
                return From(loadable, ((FileInfo)anything).FullName);

            if (anything is Stream)
                return loadable((Stream)anything);

            if (anything is byte[])
                return From(loadable, (byte[])anything);

            return default(T);
        }
    }
}
