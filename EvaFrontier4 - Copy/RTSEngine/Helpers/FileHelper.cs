using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;

namespace RTSEngine.Helpers
{
    public static class FileHelper
    {
        #region Create Game Content File
        /// <summary>
        /// Create game content file, will create file if it does not exist.
        /// Else the existing file should be loaded.
        /// </summary>
        /// <param name="relativeFilename">Relateive filename.</param>
        /// <param name="createNew">Create new file.</param>
        /// <returns>FileStream</returns>
        public static FileStream CreateGameContentFile(string relativeFilename, bool createNew) {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, relativeFilename);
            return File.Open(fullPath,
                             createNew ? FileMode.Create : FileMode.OpenOrCreate,
                             FileAccess.Write, FileShare.ReadWrite);
        }
        #endregion

        #region Load Game Content File
        /// <summary>
        /// Load game content file, returns null if file was not found.
        /// </summary>
        /// <param name="relativeFilename">Relateive filename.</param>
        /// <param name="createNew">Create new file.</param>
        /// <returns>FileStream</returns>
        public static FileStream LoadGameContentFile(string relativeFilename) {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, relativeFilename);
            if (File.Exists(fullPath) == false) {
                return null;
            } else {
                return File.Open(fullPath,
                                 FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
        }
        #endregion

        #region Save Game Content File
        /// <summary>
        /// Save game content file, returns the open file.
        /// </summary>
        /// <param name="relativeFilename">Relative filename</param>
        /// <returns>FileStream</returns>
        public static FileStream SaveGameContentFile(string relativeFilename) {
            string fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, relativeFilename);
            return File.Open(fullPath,
                             FileMode.Create, FileAccess.Write);
        }
        #endregion

        #region Open Or Create File For Current Player
        /*public static FileStream OpenFileForeCurrentPlayer(string filename, FileMode mode, FileAccess access) {
            StorageContainer container = null;
            string fullFilename = Path.Combine(container.)
        }*/
        #endregion
    }
}
