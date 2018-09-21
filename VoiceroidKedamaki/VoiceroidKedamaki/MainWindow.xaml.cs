using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VoiceroidKedamaki
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private const int HEADER_SIZE = 44;
        private SoundPlayer player = new SoundPlayer();
        private Guid operationID = Guid.Empty;
        private ImageSource[] charaImage = new ImageSource[] {
            Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.maki.GetHbitmap(),IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()),
            Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.maki2.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
        };
        private readonly Stream[] catalog = new Stream[] {
                Properties.Resources.voice_0,
                Properties.Resources.voice_1,
                Properties.Resources.voice_2,
                Properties.Resources.voice_3,
                Properties.Resources.voice_4,
                Properties.Resources.voice_5,
            };
        private readonly Dictionary<String, Stream> kana50 = new Dictionary<string, Stream>()
        {
            {"あ", Properties.Resources.kana_0},
            {"い", Properties.Resources.kana_1},
            {"う", Properties.Resources.kana_2},
            {"え", Properties.Resources.kana_3},
            {"お", Properties.Resources.kana_4},
            {"ぁ", Properties.Resources.kana_0},
            {"ぃ", Properties.Resources.kana_1},
            {"ぅ", Properties.Resources.kana_2},
            {"ぇ", Properties.Resources.kana_3},
            {"ぉ", Properties.Resources.kana_4},

            {"か", Properties.Resources.kana_5},
            {"き", Properties.Resources.kana_6},
            {"く", Properties.Resources.kana_7},
            {"け", Properties.Resources.kana_8},
            {"こ", Properties.Resources.kana_9},

            {"さ", Properties.Resources.kana_10},
            {"し", Properties.Resources.kana_11},
            {"す", Properties.Resources.kana_12},
            {"せ", Properties.Resources.kana_13},
            {"そ", Properties.Resources.kana_14},

            {"た", Properties.Resources.kana_15},
            {"ち", Properties.Resources.kana_16},
            {"つ", Properties.Resources.kana_17},
            {"て", Properties.Resources.kana_18},
            {"と", Properties.Resources.kana_19},

            {"な", Properties.Resources.kana_20},
            {"に", Properties.Resources.kana_21},
            {"ぬ", Properties.Resources.kana_22},
            {"ね", Properties.Resources.kana_23},
            {"の", Properties.Resources.kana_24},

            {"は", Properties.Resources.kana_25},
            {"ひ", Properties.Resources.kana_26},
            {"ふ", Properties.Resources.kana_27},
            {"へ", Properties.Resources.kana_28},
            {"ほ", Properties.Resources.kana_29},

            {"ま", Properties.Resources.kana_30},
            {"み", Properties.Resources.kana_31},
            {"む", Properties.Resources.kana_32},
            {"め", Properties.Resources.kana_33},
            {"も", Properties.Resources.kana_34},

            {"や", Properties.Resources.kana_35},
            {"ゆ", Properties.Resources.kana_36},
            {"よ", Properties.Resources.kana_37},
            {"ゃ", Properties.Resources.kana_35},
            {"ゅ", Properties.Resources.kana_36},
            {"ょ", Properties.Resources.kana_37},

            {"ら", Properties.Resources.kana_38},
            {"り", Properties.Resources.kana_39},
            {"る", Properties.Resources.kana_40},
            {"れ", Properties.Resources.kana_41},
            {"ろ", Properties.Resources.kana_42},

            {"わ", Properties.Resources.kana_43},
            {"を", Properties.Resources.kana_44},
            {"ん", Properties.Resources.kana_45},


            {"が", Properties.Resources.gana_0},
            {"ぎ", Properties.Resources.gana_1},
            {"ぐ", Properties.Resources.gana_2},
            {"げ", Properties.Resources.gana_3},
            {"ご", Properties.Resources.gana_4},

            {"ざ", Properties.Resources.gana_5},
            {"じ", Properties.Resources.gana_6},
            {"ず", Properties.Resources.gana_7},
            {"ぜ", Properties.Resources.gana_8},
            {"ぞ", Properties.Resources.gana_9},

            {"だ", Properties.Resources.gana_10},
            {"ぢ", Properties.Resources.gana_11},
            {"づ", Properties.Resources.gana_12},
            {"で", Properties.Resources.gana_13},
            {"ど", Properties.Resources.gana_14},
        };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (player != null) player.Stop();
                if (textbox.Text.Trim().Length > 90 || textbox.Text.Trim().Length == 0)
                {
                    throw new Exception("入力できる文章は90文字迄です。\n");
                }

                // 再生
                operationID = Guid.NewGuid();
                player.Stream = ByIME();
                image.Source = charaImage[1];
                player.Play();

                // 顔
                Task.Run(() => RevertAsync(operationID, player.Stream.Length / 2 / 44100 + 0.5));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async Task RevertAsync(Guid id, double sec)
        {
            await Task.Delay((int)(sec*1000));
            if (operationID == id)
                Dispatcher.Invoke(new Action(Revert));
               
        }

        private void Revert()
        {
            image.Source = charaImage[0];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try {
                image.Source = charaImage[0];
                operationID = Guid.Empty;
                player.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private Stream ByHash()
        {

            // 半固定乱数生成
            SHA256 sha256 = SHA256.Create();
            byte[] seed = Encoding.UTF8.GetBytes(textbox.Text);
            List<byte> hash = new List<byte>();
            while (hash.Count < 1000)
            {
                seed = sha256.ComputeHash(seed);
                hash.AddRange(seed);
            }

            // 再生音声のリストアップ
            List<Stream> srcs = new List<Stream>();
            for (int i = 0; i < textbox.Text.Length / 4 + 1; i++)
            {
                srcs.Add(catalog[hash[i] % catalog.Length]);
            }

            Stream stream = Concat(srcs);
            return stream;
        }

        private Stream ByIME()
        {
            IFELanguage fel = Activator.CreateInstance(Type.GetTypeFromProgID("MSIME.Japan")) as IFELanguage;
            fel.Open();
            string kana = fel.GetPhonetic(textbox.Text, 1, -1);
            fel.Close();
            if (kana == null) throw new Exception("文章が長すぎるか、平仮名に変換できません。\n短くシンプルな文章に変更してください。\n\n");

            List<Stream> srcs = new List<Stream>();
            foreach(char c in kana)
            {
                if (kana50.ContainsKey(c.ToString()))
                {
                    srcs.Add(kana50[c.ToString()]);
                }
            }

            if (srcs.Count <= 0)
                srcs.Add(kana50["ん"]);

            return Concat(srcs);
        }

        private static Stream Concat(List<Stream> srcs)
        {
            // 音声接続
            Stream stream = new MemoryStream();
            var headerBuf = new byte[HEADER_SIZE];
            srcs[0].Read(headerBuf, 0, HEADER_SIZE);
            stream.Write(headerBuf, 0, headerBuf.Length);
            foreach (var src in srcs)
            {
                var dataBuf = new byte[src.Length - HEADER_SIZE];
                src.Position = HEADER_SIZE;
                src.Read(dataBuf, 0, dataBuf.Length);
                src.Position = 0;
                stream.Write(dataBuf, 0, dataBuf.Length);
            }
            stream.Position = 4; // wav size;
            stream.Write(BitConverter.GetBytes((Int32)stream.Length - 8), 0, 4);
            stream.Position = HEADER_SIZE - 4; // data size;
            stream.Write(BitConverter.GetBytes((Int32)stream.Length - HEADER_SIZE), 0, 4);

            stream.Position = 0;
            return stream;
        }

        [ComImport]
        [Guid("019F7152-E6DB-11D0-83C3-00C04FDDB82E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IFELanguage
        {
            void Open();
            void Close();
            void Dummy5(); /* DO NOT CALL */
            void Dummy6(); /* DO NOT CALL */
            [return: MarshalAs(UnmanagedType.BStr)]
            string GetPhonetic([MarshalAs(UnmanagedType.BStr)] string str, int start, int length);
            void Dummy8(); /* DO NOT CALL */
        }
    }
}
