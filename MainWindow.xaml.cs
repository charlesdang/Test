using Codeplex.Data;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp5
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //using (var ftp = new FtpClient("192.168.1.3"))
            //{
            //    ftp.Connect();

            //    ftp.CreateDirectory("/temp/test2");

            //    // upload a file to an existing FTP directory
            //    ftp.UploadFile(@"C:\Users\Charles\Documents\文字文稿1.docx", "/temp/test2/1.docx");

            //    // upload a file and ensure the FTP directory is created on the server
            //    ftp.UploadFile(@"C:\Users\Charles\Desktop\无标题.png", "/temp/1.png", FtpRemoteExists.Overwrite, true);

            //    // upload a file and ensure the FTP directory is created on the server, verify the file after upload
            //    ftp.UploadFile(@"C:\Users\Charles\Desktop\无标题.png", "/temp/1.png", FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            //}

            // 将Json字符串解析成DynamicJson对象
            var json = DynamicJson.Parse(@"{""foo"":""json"", ""bar"":100, ""nest"":{ ""foobar"":true } }");

            var r1 = json.foo; // "json" - string类型
            var r2 = json.bar; // 100 - double类型
            var r3 = json.nest.foobar; // true - bool类型
            var r4 = json["nest"]["foobar"]; // 还可以和javascript一样通过索引器获取

            // 判断json字符串中是否包含指定键
            var b1_1 = json.IsDefined("foo"); // true
            var b2_1 = json.IsDefined("foooo"); // false
            // 上面的判断还可以更简单，直接通过json.键()就可以判断
            var b1_2 = json.foo(); // true
            var b2_2 = json.foooo(); // false;


            // 新增操作
            json.Arr = new string[] { "NOR", "XOR" }; // 新增一个js数组
            json.Obj1 = new { }; // 新增一个js对象
            json.Obj2 = new { foo = "abc", bar = 100 }; // 初始化一个匿名对象并添加到json字符串中

            // 删除操作
            json.Delete("foo");
            json.Arr.Delete(0);
            // 还可以更简单去删除，直接通过json(键); 即可删除。
            json("bar");
            json.Arr(1);

            // 替换操作
            json.Obj1 = 5000;

            // 创建一个新的JsonObject
            dynamic newjson = new DynamicJson();
            newjson.str = "aaa";
            newjson.obj = new { foo = "bar" };

            // 直接序列化输出json字符串
            var jsonstring = newjson.ToString(); // {"str":"aaa","obj":{"foo":"bar"}}


            var objectJson = DynamicJson.Parse(@"{""foo"":""json"",""bar"":100}");
            foreach (KeyValuePair<string, dynamic> item in objectJson)
            {
                Console.WriteLine(item.Key + ":" + item.Value); // foo:json, bar:100
            }
          
            var objectJsonList = DynamicJson.Parse(@"[{""bar"":50},{""bar"":100}]");
            //var barSum = ((FooBar[])objectJsonList).Select(fb => fb.bar).Sum(); // 150
            var dynamicWithLinq = ((dynamic[])objectJsonList).Select(d => d.bar);

            FileStream fs = new FileStream(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"json", "API_Classify.json"), FileMode.Open, FileAccess.Read);
            var jsonTest = DynamicJson.Parse(fs, Encoding.UTF8);
            fs.Dispose();
            var algoTypes = ((dynamic[])jsonTest.algorithm).Select(d => d.algoType);
            var parames = ((dynamic[])jsonTest.algorithm).Select(d => d.param);
            var defectMergeNode = ((dynamic[])jsonTest.algorithm).FirstOrDefault(d => d.algoType == "defectMerge");
            var p2 = "param";
            var p3 = "flag";
            var p4 = "value";
            if (defectMergeNode.param.IsDefined("flag"))
                defectMergeNode.param.flag.value = "2";
            if (defectMergeNode.param.IsDefined(p3))
                defectMergeNode[p2][p3][p4] = "44";
            foreach (KeyValuePair<string, dynamic> item in jsonTest)
            {
                Console.WriteLine(item.Key + ":" + item.Value); // foo:json, bar:100

                //var jsonTestSub = item.Value as DynamicJson;
                if(item.Value.IsArray)
                {
                    var algorithms = (dynamic[])DynamicJson.Parse(item.Value.ToString());
                    //var keys = jsonTestSub.Select(s=>s.algoType);
                    foreach (var algorithm in algorithms)
                    {
                        //var algorithmObj = (DynamicJson)algorithm;
                        if (algorithm.IsObject)
                        {
                            if (algorithm.IsDefined("algoType"))
                            {
                                Console.WriteLine("algoType:"+ algorithm.algoType);
                            }
                            if (algorithm.IsDefined("param"))
                            {
                                //Console.WriteLine("param:" + algorithm.param);

                                if(algorithm.param.IsObject)
                                {
                                    foreach (KeyValuePair<string, dynamic> paramSub in algorithm.param)
                                    {
                                        Console.WriteLine("paramName:" + paramSub.Key);
                                        if(paramSub.Value.IsDefined("value"))
                                            Console.WriteLine("paramValue:" + paramSub.Value.value);
                                        if (paramSub.Value.IsDefined("introdution"))
                                            Console.WriteLine("paramDesc:" + paramSub.Value.introdution);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
