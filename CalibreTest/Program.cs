using System.IO;
using System.Diagnostics;
//ebook-convert dosya.pdf dosya.epub --prefer-metadata-cover --no-chapters-in-toc --epub-version 3 --no-default-epub-cover --extra-css p{text-indent:5mm;}
// try catch lere al
// boş bir projede kitapı farklı yere bastırıp unzip yaptır.
class Program
{
    static string epubPath = @"C:\Users\Gurkan\Desktop\epub\";
    static string pdfPath = @"C:\Users\Gurkan\Desktop\pdf\";
    static string unzipPdfPath = @"C:\Users\Gurkan\Desktop\pdf\karabalik.pdf";
    static string extractPath = @"C:\Users\Gurkan\Desktop\extract\";
    static void Main(string[] args)
    {


        //FileSystemWatcher adlı sınıftan bir örnek alıyoruz ve izleyeceğimiz yoluda constructor'da yolu giriyoruz.
        var file = new FileSystemWatcher(@"C:\Users\Gurkan\Desktop\pdf");

        //Alt kategorileri izlemeye dahil et.
        file.IncludeSubdirectories = true;

        file.Changed += (sender, e) => {
            ConvertToEpub(e.FullPath);
        };
        file.Created += (sender, e) => {
            ConvertToEpub(e.FullPath);
        };

        //Bir hata durumunda tetiklenecek event.
        file.Error += (sender, eventArgs) => {
            Console.WriteLine($"Hata: {eventArgs.GetException()}");
        };

        // İzleme işlemini başlatıyoruz.
        file.EnableRaisingEvents = true;

        ConvertToEpub(pdfPath);
    }

    static void ConvertToEpub(string pdfpath)
    {
        //if (Path.GetExtension(pdfpath).ToLower()!=".pdf")
        //    return;
        try
        {
            //Console.WriteLine(Guid.NewGuid().ToString());
            //return;
            string isbn = Path.GetFileNameWithoutExtension(unzipPdfPath);
            Console.WriteLine(isbn);
            string savePath = Path.Combine(epubPath, isbn);
            string unzipPath = Path.Combine(savePath+".epub");
            Console.WriteLine(unzipPath);
            string myPath = "CMD.exe";
            string query = string.Format("/c ebook-convert {0}karabalik.pdf {1}.epub --prefer-metadata-cover --no-chapters-in-toc --no-default-epub-cover --epub-version {2} --extra-css \"{3}\"", pdfpath, savePath,
                    3, "p { text-indent:5mm; }");
            Process prc = new();
            prc.StartInfo.FileName = myPath;
            prc.StartInfo.Arguments = query;
            Console.WriteLine(query);
            prc.Start();
            Console.WriteLine("bitti");
            prc.WaitForExit();
            System.IO.Compression.ZipFile.ExtractToDirectory(unzipPath, extractPath);


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.ReadLine();

    }
}
