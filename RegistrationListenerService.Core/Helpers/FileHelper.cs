using CsvHelper;
using RegistrationListenerService.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Helpers {
    internal static class FileHelper {

        /// <summary>
        /// Writes a RegistrationMessage record to a file. If the directory or the file does not exist,
        /// it creates them and then appends the record to the file.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        /// <param name="message"></param>
        internal static void WriteToCSVFile(string directory, string filename, RegistrationMessage message) {
            
            if (!filename.EndsWith(".csv")) {
                filename += ".csv";
            }            
            string fullPath = Path.Combine(directory, filename);

            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }            

            if (!File.Exists(fullPath)) {                
                using (var textWriter = File.CreateText(fullPath)) {                    
                    var writer = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
                    
                    foreach (var property in message.GetType().GetProperties()) {
                        writer.WriteField(property.Name);
                    }
                    writer.NextRecord();
                }
            }

            WriteFile(fullPath, message);
        }

        /// <summary>
        /// Helper method to append a RegistrationMessage record to the file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="message"></param>
        private static void WriteFile(string fullPath, RegistrationMessage message) {

            using (var stream = File.Open(fullPath, FileMode.Append))
            using (var textWriter = new StreamWriter(stream)){
                var writer = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
                
                foreach (var property in message.GetType().GetProperties()) {
                    writer.WriteField(property.GetValue(message));
                }
                writer.NextRecord();
            }
        }
    }
}
