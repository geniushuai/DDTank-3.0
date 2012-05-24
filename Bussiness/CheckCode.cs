using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Bussiness
{
    public class CheckCode
    {
        #region new 

        #region property

        public static ThreadSafeRandom rand = new ThreadSafeRandom();

        //定义颜色
        private static Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
        //定义字体
        private static string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

         //以数组方式候选字符，可以更方便的剔除不要的字符，如数字 0 与字母 o
        private static char[] digitals = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private static char[] lowerLetters = new char[] {
                'a', 'b', 'c', 'd', 'e', 'f',  
                'h', 'k', 'm', 'n', 
                 'p', 'q', 'r', 's', 't', 
                'u', 'v', 'w', 'x', 'y', 'z' };

        private static char[] upperLetters = new char[] {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                'H', 'K', 'M', 'N', 
                'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z' };

        private static char[] letters = new char[]{
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 
                'h', 'i', 'j', 'k', 'l', 'm', 'n', 
                 'p', 'q', 'r', 's', 't', 
                'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                'H', 'I', 'J', 'K', 'L', 'M', 'N', 
                 'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z' };

        private static char[] mix = new char[]{
                '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 
                'h', 'k', 'm', 'n', 
                 'p', 'q', 'r', 's', 't', 
                'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 
                'H', 'K', 'M', 'N', 
                 'P', 'Q', 'R', 'S', 'T', 
                'U', 'V', 'W', 'X', 'Y', 'Z' };

        private enum RandomStringMode
        {
            /// <summary>
            /// 小写字母
            /// </summary>
            LowerLetter,
            /// <summary>
            /// 大写字母
            /// </summary>
            UpperLetter,
            /// <summary>
            /// 混合大小写字母
            /// </summary>
            Letter,
            /// <summary>
            /// 数字
            /// </summary>
            Digital,
            /// <summary>
            /// 混合数字与大小字母
            /// </summary>
            Mix
        }

        #endregion

         ///  <summary>
        ///  创建随机码图片
        ///  </summary>
        ///  <param  name="randomcode">随机码</param>
        public static byte[] CreateImage(string randomcode)
        {
            int randAngle = 30; //随机转动角度
            int mapwidth = (int)(randomcode.Length * 30);
            Bitmap map = new Bitmap(mapwidth, 36);//创建图片背景
            Graphics graph = Graphics.FromImage(map);
            try
            {

                graph.Clear(Color.White);//清除画面，填充背景


                int cindex = rand.Next(7);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);

                for (int i = 0; i < 1; i++)
                {
                    int x1 = rand.Next(map.Width / 2);
                    int x2 = rand.Next(map.Width * 3 / 4, map.Width);
                    int y1 = rand.Next(map.Height);
                    int y2 = rand.Next(map.Height);

                    graph.DrawBezier(new Pen(c[cindex], 2), x1, y1, (x1 + x2) / 4, 0, (x1 + x2) * 3 / 4, map.Height, x2, y2);
                }


                //验证码旋转，防止机器识别
                char[] chars = randomcode.ToCharArray();//拆散字符串成单字符数组

                //文字距中
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                for (int i = 0; i < chars.Length; i++)
                {
                    int findex = rand.Next(5);

                    Font f = new System.Drawing.Font(font[findex], 22, System.Drawing.FontStyle.Bold);//字体样式(参数2为字体大小)

                    Point dot = new Point(16, 16);
                    //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);//测试X坐标显示间距的
                    float angle = ThreadSafeRandom.NextStatic(-randAngle, randAngle);//转动的度数

                    graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                    graph.RotateTransform(angle);
                    graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                    graph.RotateTransform(-angle);//转回去
                    graph.TranslateTransform(2, -dot.Y);//移动光标到指定位置
                }

                //生成图片
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            finally
            {
                graph.Dispose();
                map.Dispose();
            }
        }

        private static string GenerateRandomString(int length, RandomStringMode mode)
        {
            string rndStr = string.Empty;
            if (length == 0)
                return rndStr;         

            int[] range = new int[2] { 0, 0 };

            switch (mode)
            {
                case RandomStringMode.Digital:
                    for (int i = 0; i < length; ++i)
                        rndStr += digitals[rand.Next(0, digitals.Length)];
                    break;

                case RandomStringMode.LowerLetter:
                    for (int i = 0; i < length; ++i)
                        rndStr += lowerLetters[rand.Next(0, lowerLetters.Length)];
                    break;

                case RandomStringMode.UpperLetter:
                    for (int i = 0; i < length; ++i)
                        rndStr += upperLetters[rand.Next(0, upperLetters.Length)];
                    break;

                case RandomStringMode.Letter:
                    for (int i = 0; i < length; ++i)
                        rndStr += letters[rand.Next(0, letters.Length)];
                    break;

                default:
                    for (int i = 0; i < length; ++i)
                        rndStr += mix[rand.Next(0, mix.Length)];
                    break;
            }

            return rndStr;
        }

        public static string GenerateCheckCode()
        {
            return GenerateRandomString(4, RandomStringMode.Mix);
        }

        #endregion


        #region order

        //public static ThreadSafeRandom random = new ThreadSafeRandom();

        //public static Color[] colors = new Color[] { Color.Blue, Color.DarkRed, Color.Gray, Color.Gold,Color.Yellow,Color.Orange,Color.Silver };
        ////public static Color[] colors1 = new Color[] { Color.DarkRed, Color.Blue, Color.Gold, Color.Red };
        //public static LinearGradientMode[] linears = new LinearGradientMode[] { LinearGradientMode.BackwardDiagonal, LinearGradientMode.ForwardDiagonal, LinearGradientMode.Horizontal, LinearGradientMode.Vertical };
        //public static int[] colorss = new int[] { 0xff5b33, 0x3360ff, 0xe633ff, 0x33ffed, 0xbf0b7a, 0x009037, 0x002790, 0xff61ac, 0xf78400, 0x8f08ff };

        //public static string GenerateCheckCode()
        //{
        //    int number;
        //    char code;
        //    string checkCode = String.Empty;

        //    System.Random random = new Random();

        //    for (int i = 0; i < 2; i++)
        //    {
        //        number = random.Next();

        //        //if (number % 2 == 0)
        //        //    code = (char)('0' + (char)(number % 10));
        //        //else
        //        code = (char)('A' + (char)(number % 26));

        //        checkCode += code.ToString();
        //    }

        //    return checkCode;
        //}

        //public static string[] GenerateCheckCodes(int count)
        //{
        //    int number;
        //    char code;
        //    string codes;
        //    //string[] checkCodes = new string[count];

        //    //for (int k = 0; k < count; k++)
        //    //{
        //    //    for (int i = 0; i < 2; i++)
        //    //    {
        //    //        number = random.Next();
        //    //        code = (char)('A' + (char)(number % 26));
        //    //        checkCodes[k] += code.ToString();
        //    //    }
        //    //}

        //    List<string> checkCodes = new List<string>();
        //    while (checkCodes.Count < count)
        //    {
        //        codes = string.Empty;
        //        for (int i = 0; i < 2; i++)
        //        {
        //            number = random.Next();
        //            code = (char)('A' + (char)(number % 26));
        //            codes += code.ToString();
        //        }
        //        if (!checkCodes.Contains(codes))
        //        {
        //            checkCodes.Add(codes);
        //        }
        //    }

        //    return checkCodes.ToArray();
        //}

        //public static byte[] CreateCheckCodeImage(string checkCode)
        //{
        //    if (checkCode == null || checkCode.Trim() == String.Empty)
        //        return null;


        //    System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 30.5)), 44);
        //    Graphics g = Graphics.FromImage(image);

        //    try
        //    {
        //        //生成随机生成器 
        //        Random random = new Random();
        //        //int rand = random.Next(colors.Length);
        //        Color color = colors[random.Next(colors.Length)];
        //        Color color1 = colors[random.Next(colors.Length)];
        //        LinearGradientMode linear = linears[random.Next(linears.Length)];

        //        //清空图片背景色 
        //        //g.Clear(Color.White);
        //        g.Clear(Color.Transparent);

        //        for (int i = 0; i < 2; i++)
        //        {
        //            int x1 = random.Next(image.Width);
        //            //int x2 = random.Next(image.Width);
        //            int y1 = random.Next(image.Height);
        //            //int y2 = random.Next(image.Height);

        //            g.DrawArc(new Pen(color, 2), -x1, -y1, image.Width * 2, image.Height, 45, 100);

        //        }

        //        Font font = new System.Drawing.Font("Arial", 24, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
        //        System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), color, color1, linear);
        //        g.DrawString(checkCode, font, brush, 2, 2);

        //        //画图片的前景噪音点 
        //        //for (int i = 0; i < 100; i++)
        //        //{
        //        //    int x = random.Next(image.Width);
        //        //    int y = random.Next(image.Height);

        //        //    image.SetPixel(x, y, Color.FromArgb(random.Next()));
        //        //}

        //        ////画图片的边框线 
        //        //g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);


        //        //image.Save(string.Format("1.bmp"));
        //        //image.Save("c:\\1.bmp");


        //        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        //        return ms.ToArray();
        //        //Response.ClearContent();
        //        //Response.ContentType = "image/Gif";
        //        //Response.BinaryWrite(ms.ToArray());
        //    }
        //    finally
        //    {
        //        g.Dispose();
        //        image.Dispose();
        //    }
        //}

        //public static object[] CreateRegionCode(int strlength)
        //{
        //    //定义一个字符串数组储存汉字编码的组成元素 
        //    string[] rBase = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

        //    Random rnd = new Random();

        //    //定义一个object数组用来 
        //    object[] bytes = new object[strlength];

        //    /**/
        //    /*每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bject数组中 
        // 每个汉字有四个区位码组成 
        // 区位码第1位和区位码第2位作为字节数组第一个元素 
        // 区位码第3位和区位码第4位作为字节数组第二个元素 
        //*/
        //    for (int i = 0; i < strlength; i++)
        //    {
        //        //区位码第1位 
        //        int r1 = rnd.Next(11, 14);
        //        string str_r1 = rBase[r1].Trim();

        //        //区位码第2位 
        //        rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i);//更换随机数发生器的 


        //        int r2;
        //        if (r1 == 13)
        //        {
        //            r2 = rnd.Next(0, 7);
        //        }
        //        else
        //        {
        //            r2 = rnd.Next(0, 16);
        //        }
        //        string str_r2 = rBase[r2].Trim();

        //        //区位码第3位 
        //        rnd = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
        //        int r3 = rnd.Next(10, 16);
        //        string str_r3 = rBase[r3].Trim();

        //        //区位码第4位 
        //        rnd = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
        //        int r4;
        //        if (r3 == 10)
        //        {
        //            r4 = rnd.Next(1, 16);
        //        }
        //        else if (r3 == 15)
        //        {
        //            r4 = rnd.Next(0, 15);
        //        }
        //        else
        //        {
        //            r4 = rnd.Next(0, 16);
        //        }
        //        string str_r4 = rBase[r4].Trim();

        //        //定义两个字节变量存储产生的随机汉字区位码 
        //        byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
        //        byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
        //        //将两个字节变量存储在字节数组中 
        //        byte[] str_r = new byte[] { byte1, byte2 };

        //        //将产生的一个汉字的字节数组放入object数组中 
        //        bytes.SetValue(str_r, i);

        //    }

        //    return bytes;

        //}

        #endregion
    }
}
