using System.IO;
using System.Diagnostics;
class Program
{
    static string epubpath = @"C:\Users\Gurkan\Desktop\epub\";
    static string pdfpath = @"C:\Users\Gurkan\Desktop\pdf\";
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

        ConvertToEpub(pdfpath);
    }

    static void ConvertToEpub(string pdfpath)
    {
        //if (Path.GetExtension(pdfpath).ToLower()!=".pdf")
        //    return;
        try
        {
            //string fileName = Path.GetFileNameWithoutExtension(pdfpath);
            //Path.Combine(epubpath, fileName);
            string myPath = "CMD.exe";
            string query = string.Format("/c ebook-convert {0}karabalik.pdf {1}dosya.epub --prefer-metadata-cover --no-chapters-in-toc --no-default-epub-cover --epub-version {2} --extra-css \"{3}\"", pdfpath, epubpath,
                    3, "p { text-indent:5mm; }");
            Process prc = new();
            prc.StartInfo.FileName = myPath;
            prc.StartInfo.Arguments = query;//query
            Console.WriteLine(query);
            prc.Start();
            Console.WriteLine("bitti");
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.ReadLine();
      
 

        //ebook-convert dosya.pdf dosya.epub --prefer-metadata-cover --no-chapters-in-toc --epub-version 3 --no-default-epub-cover --extra-css p{text-indent:5mm;}
    }
}
