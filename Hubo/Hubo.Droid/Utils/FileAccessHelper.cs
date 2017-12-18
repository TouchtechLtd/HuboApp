// <copyright file="FileAccessHelper.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid
{
    using System.IO;

    public static class FileAccessHelper
    {
        public static string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string dbPath = Path.Combine(path, filename);

            CopyDatabaseIfNotExists(dbPath);

            return dbPath;
        }

        private static void CopyDatabaseIfNotExists(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                using (var br = new BinaryReader(Android.App.Application.Context.Assets.Open("Hubo.db3")))
                {
                    using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int length;
                        while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, length);
                        }
                    }
                }
            }
        }
    }
}