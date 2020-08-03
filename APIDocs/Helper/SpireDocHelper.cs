using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Spire.Doc;
using Spire.Doc.Documents;
using System;
using System.IO;

namespace APIDocs.Helper
{
    public class SpireDocHelper
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SpireDocHelper(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public Stream SwaggerHtmlConvers(string html, string type, out string memi)
        {
            string fileName = $"生成接口word文档-{TimeParser.GetUnixTimestamp(DateTime.Now)}{type}";
            string webRootPath = _hostingEnvironment.ContentRootPath;
            string path = webRootPath + @"\TempFiles\";
            var addrUrl = path + $"{fileName}";
            var provider = new FileExtensionContentTypeProvider();
            memi = provider.Mappings[type];
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var data = System.Text.Encoding.Default.GetBytes(html);
                //byte[] data = Encoding.Default.getbyte(ByteHelpe.html);
                using Stream stream = new MemoryStream(data);

                //创建Document实例
                using Document document = new Document();

                //加载HTML文档
                //document.LoadFromFile("APIDocument.html", FileFormat.Html, XHTMLValidationType.None);
                document.LoadFromStream(stream, FileFormat.Html, XHTMLValidationType.None);
                //document.LoadText(stream, Encoding.Default);

                //保存为Word
                switch (type)
                {
                    case ".docx":
                        //Word
                        document.SaveToFile(addrUrl, FileFormat.Docx);
                        break;

                    case ".pdf":
                        //PDF
                        document.SaveToFile(addrUrl, FileFormat.PDF);
                        break;

                    case ".html":
                        //Html
                        FileStream fs = new FileStream(addrUrl, FileMode.Append, FileAccess.Write,
                            FileShare.None); //html直接写入不用spire.doc
                        StreamWriter sw = new StreamWriter(fs); // 创建写入流
                        sw.WriteLine(html); // 写入Hello World
                        sw.Close(); //关闭文件
                        fs.Close();
                        break;

                    case ".xml":
                        //PDF
                        document.SaveToFile(addrUrl, FileFormat.WordXml);
                        break;

                    case ".svg":
                        //PDF
                        document.SaveToFile(addrUrl, FileFormat.SVG);
                        break;
                }

                document.Close();
                using var fileStream = System.IO.File.Open(addrUrl, FileMode.OpenOrCreate);
                var filedata = ByteHelper.StreamToBytes(fileStream);
                var outdata = ByteHelper.BytesToStream(filedata);
                return outdata;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //if (System.IO.File.Exists(addrUrl))
                //    System.IO.File.Delete(addrUrl); //删掉文件
            }
        }
    }

    public class ByteHelper
    {
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始

            stream.Seek(0, SeekOrigin.Begin);

            return bytes;
        }

        /// <summary>
        /// 将 byte[] 转成 Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);

            return stream;
        }
    }
}