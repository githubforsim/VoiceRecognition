using System;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Collections.Generic;

namespace VoiceRecognition
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine();

        List<string> _mots = new List<string>()
            {
                "voiture",
                "chat",
                "machine", 
                "avant", "arrière",
                "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf",
                "démarrer", "arrêter",
                "ouvrir", "fermer",
                "aime aile une", /*ML01*/
                "aime aile deux", /*ML02*/
                "sait pé soixante", /*CP60*/
            };

        public Form1()
        {
            InitializeComponent();
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Choices commands = new Choices();
            commands.Add(_mots.ToArray());
            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands);
            Grammar grammar = new Grammar(gBuilder);

            recEngine.LoadGrammarAsync(grammar);
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += RecEngine_SpeechRecognized; 
        }

        private void RecEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            logCtrl.Items.Add(e.Result.Text);
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsyncStop();
        }
    }
}
