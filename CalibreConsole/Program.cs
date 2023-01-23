using System.IO;
using System.Diagnostics;
using Npgsql;
//ebook-convert dosya.pdf dosya.epub --prefer-metadata-cover --no-chapters-in-toc --epub-version 3 --no-default-epub-cover --extra-css p{text-indent:5mm;}
class Program
{
    static string epubPath = @"C:\Users\Gurkan\Desktop\epub\";
    //static string pdfPath = @"C:\Users\Gurkan\Desktop\pdf\";
    //static string extractPath = @"C:\Users\Gurkan\Desktop\extract";
    static void Main(string[] args)
    {


        //FileSystemWatcher adlı sınıftan bir örnek alıyoruz ve izleyeceğimiz yoluda constructor'da yolu giriyoruz.
        var file = new FileSystemWatcher(@"C:\Users\Gurkan\Desktop\pdf");

        //Alt kategorileri izlemeye dahil et.
        file.IncludeSubdirectories = true;

        file.Changed += (sender, e) =>
        {

            if (File.Exists(e.FullPath))
                ConvertToEpub(e.FullPath);
        };
        file.Created += (sender, e) =>
        {
            ConvertToEpub(e.FullPath);
        };

        //Bir hata durumunda tetiklenecek event.
        file.Error += (sender, eventArgs) =>
        {
            Console.WriteLine($"Hata: {eventArgs.GetException()}");
        };

        // İzleme işlemini başlatıyoruz.
        file.EnableRaisingEvents = true;
        Console.ReadLine();
    }

    static void KeepLog(string message, string level = "Error")
    {
        try
        {
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost; Port=5432; User Id=postgres; Password=1; Database=Kitap12Logs"))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand("select public.loginsertselect(@date, @message, @level, @assembly, '', '', '')", conn);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@message", message);
                cmd.Parameters.AddWithValue("@level", level);
                cmd.Parameters.AddWithValue("@assembly", "Calibre Convert");
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    static void ConvertToEpub(string pdfpath)
    {
        //if (Path.GetExtension(pdfpath).ToLower()!=".pdf")
        //    return;
        try
        {
            //Console.WriteLine(Guid.NewGuid().ToString());

            //return;
            string isbn = Path.GetFileNameWithoutExtension(pdfpath);
            Console.WriteLine(isbn);
            string id = GetBookId(isbn);
            if (id != null)
            {

                string saveDir = Path.Combine(epubPath, id);
                string savePath = Path.Combine(saveDir, isbn) + ".epub";
                string myPath = "CMD.exe";
                string query = string.Format("/c ebook-convert {0} {1} --prefer-metadata-cover --no-chapters-in-toc --no-default-epub-cover --epub-version {2} --extra-css \"{3}\"", pdfpath, savePath,
                        3, "p { text-indent:5mm; }");
                if (!Directory.Exists(saveDir))
                    Directory.CreateDirectory(saveDir);
                Process prc = new();
                prc.StartInfo.FileName = myPath;
                prc.StartInfo.Arguments = query;
                Console.WriteLine(query);
                prc.Start();
                Console.WriteLine("bitti");
                prc.WaitForExit();
                System.IO.Compression.ZipFile.ExtractToDirectory(savePath, saveDir, true);
                File.Delete(savePath);
            }

            FileDelete(pdfpath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


        static void FileDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static string? GetBookId(string isbn)
        {
            string? id;
            using (NpgsqlConnection conn = new NpgsqlConnection("Server=localhost; Port=5432; User Id=postgres; Password=1; Database=Kitap12"))
            {
                conn.Open();

                NpgsqlCommand cmd = new NpgsqlCommand("SELECT id FROM book WHERE isbn = @filename", conn);
                cmd.Parameters.AddWithValue("@filename", isbn);
                id = cmd.ExecuteScalar()?.ToString();

                conn.Close();
            }
            if (id == null)
            {
                KeepLog(string.Format("{0} isbn numaralı kitap bulunamadı", isbn));
            }
            return id;
        }

    }
}
