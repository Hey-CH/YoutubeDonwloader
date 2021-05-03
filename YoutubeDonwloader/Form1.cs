using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using VideoLibrary;

/*
 * VideoLibraryを使用する場合、NuGetで入れた後
 * ツール＞NuGetパッケージマネージャー＞パッケージマネージャーコンソールで
 * PM> Update-Package -Reinstall
 * を実行すると良いと思います。
 * 
 * System.Net.Httpでエラーが出る場合
 * App.configを開いてSystem.Net.Httpのバージョンを4.0.0.0に設定する事
 */

namespace YoutubeDonwloader {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            textBox2.Text = Properties.Settings.Default.ffmpegPath;
            TextBoxCheck();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            Properties.Settings.Default.ffmpegPath = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
            TextBoxCheck();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            TextBoxCheck();
        }
        private void TextBoxCheck() {
            if (File.Exists(textBox2.Text) && textBox2.Text.EndsWith("ffmpeg.exe")) {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                button1.Text = "Convert";
            } else {
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                button1.Text = "Download";
            }
        }
        private void TextBoxN_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            //0～9と、バックスペース以外の時は、イベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b') {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            //Youtube動画出力
            var youTube = YouTube.Default;
            var video = youTube.GetVideo(textBox1.Text);
            System.IO.File.WriteAllBytes(video.FullName, video.GetBytes());

            //出力
            string interval = "";
            TimeSpan start_position = new TimeSpan(0, 0, int.Parse(textBox3.Text));
            try {
                TimeSpan end_position = new TimeSpan(0, 0, int.Parse(textBox4.Text));
                interval = (end_position - start_position).TotalSeconds.ToString();
            } catch (Exception ex) {
            }

            if (checkBox1.Checked == true) {
                string filePath = video.FullName + ".gif";
                if (string.IsNullOrEmpty(interval)) {
                    var arg = "-ss " + start_position.TotalSeconds.ToString()
                        + " -i \"" + video.FullName + "\""
                        + " \"" + filePath + "\"";
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(textBox2.Text , arg);
                    p.WaitForExit();
                } else {
                    var arg = "-ss " + start_position.TotalSeconds.ToString()
                        + " -i \"" + video.FullName + "\""
                        + " -t " + interval
                        + " \"" + filePath + "\"";
                    System.Diagnostics.Process p =
                    System.Diagnostics.Process.Start(
                        textBox2.Text
                        , arg);
                    p.WaitForExit();
                }
            }
            if (checkBox2.Checked == true) {
                string filePath = video.FullName+".mp3";
                if (string.IsNullOrEmpty(interval)) {
                    System.Diagnostics.Process p =
                    System.Diagnostics.Process.Start(
                        textBox2.Text
                        , "-ss " + start_position.TotalSeconds.ToString()
                        + " -i \"" + video.FullName + "\""
                        + " \"" + filePath + "\"");
                    p.WaitForExit();
                } else {
                    System.Diagnostics.Process p =
                    System.Diagnostics.Process.Start(
                        textBox2.Text
                        , "-ss " + start_position.TotalSeconds.ToString()
                        + " -i \"" + video.FullName + "\""
                        + " -t " + interval 
                        + " \"" + filePath + "\"");
                    p.WaitForExit();
                }
            }

            //完了
            MessageBox.Show("完了しました。");
        }
    }
}
