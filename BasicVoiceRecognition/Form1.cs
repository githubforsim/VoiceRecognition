using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace VoiceRecognition
{
    public partial class Form1 : Form
    {
        private UdpClient _udpClient;
        private const int _port = 12345;
        private bool _isOn = false;
        private bool _isPythonReady = false;
        private Process _voiceProcess;
        private bool _isClosing = false;

        public Form1()
        {
            InitializeComponent();
            _udpClient = new UdpClient(_port);
            _udpClient.BeginReceive(Receive, new object());
            label1.Text = "Python Off";
            label2.Text = "Disable";

            StartVoiceProcess();

            this.FormClosing += Form1_FormClosing;
        }

        private void StartVoiceProcess()
        {
            string voiceExePath = Path.Combine(Application.StartupPath, "..", "..", "..", "Voice EXE", "voice.exe");
            _voiceProcess = new Process();
            _voiceProcess.StartInfo.FileName = voiceExePath;

            try
            {
                _voiceProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du démarrage de voice.exe : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopVoiceProcess()
        {
            try
            {
                if (_voiceProcess != null && !_voiceProcess.HasExited)
                {
                    _voiceProcess.CloseMainWindow();
                    _voiceProcess.Kill();
                    _voiceProcess.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'arrêt de voice.exe : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Receive(IAsyncResult ar)
        {
            if (_isClosing)
                return;

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _port);
            byte[] bytes = _udpClient.EndReceive(ar, ref ip);
            string result = Encoding.UTF8.GetString(bytes);

            if (result.StartsWith("Type1:"))
            {
                // Message de Type 1
                string messageContent = result.Substring("Type1:".Length);
                Invoke(new Action(() => PythonReady(messageContent)));
            }
            else if (result.StartsWith("Type2:"))
            {
                // Message de Type 2
                string messageContent = result.Substring("Type2:".Length);
                Invoke(new Action(() => PythonResult(messageContent)));
            }

            _udpClient.BeginReceive(Receive, new object());
        }

        private void PythonReady(string content)
        {
            if (!_isPythonReady && content == "Ready")
            {
                label1.Text = "Python On";
                _isPythonReady = true;
            }
            else if (content == "UnReady")
            {
                label1.Text = "Python Off";
                _isPythonReady = false;
            }
        }

        private void PythonResult(string result)
        {
            if (_isOn && result != "")
                logCtrl.Items.Add(result);
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            _isOn = true;
            label2.Text = "Enable";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            _isOn = false;
            label2.Text = "Disable";
        }

        private void Form1_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            _isClosing = true;
            StopVoiceProcess();
        }
    }
}
