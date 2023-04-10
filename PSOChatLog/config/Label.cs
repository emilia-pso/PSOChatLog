using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace PSOChatLog
{
    public class Label
    {
        private Form form;

        public static string TITLE { get; } = "PSO Chat Log Ver " + Config.VERSION;

        public Label(Form form)
        {
            form = this.form;
        }

        /*
         * ラベルを定義するよ。
         * 新しいラベルができたらここで定義してね。
         */
        private static Dictionary<string, string> form1_Labels(string language_GP)
        {
            Dictionary<string, Dictionary<string, string>> map = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    Language_GP.EN,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "EtoX" },
                        { "Button2", "XtoE" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language_GP.DE,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "DtoX"},
                        { "Button2", "XtoD" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language_GP.ES,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "StoX" },
                        { "Button2", "XtoS" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language_GP.FR,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "FtoX" },
                        { "Button2", "XtoF" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language_GP.RU,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "RtoX" },
                        { "Button2", "XtoR" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language_GP.ZH_CN,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "CtoX" },
                        { "Button2", "XtoC" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language_GP.ZH_TW,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "CtoX" },
                        { "Button2", "XtoC" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language_GP.JA,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "和XX" },
                        { "Button2", "XX和" },
                        { "Button3", "送信" },
                        { "Button4", "設定" }
                    }
                }
                ,
                {
                    Language_GP.KO,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "KtoX" },
                        { "Button2", "XtoK" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
            };
            return map[language_GP];
        }

        /*
         * ラベルを定義するよ。
         * 新しいラベルができたらここで定義してね。
         */
        private static Dictionary<string, string> form2_Labels(string language_GP)
        {
            Dictionary<string, Dictionary<string, string>> map = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    Language_GP.EN,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "OK" },

                        { "tabPage1", "Install" },
                        { "label4", "Where to install Ephinea PSOBB" },
                        { "Button13", "Auto-Get from the registry (Recommend)" },
                        { "Button2", "Auto-Get from the start menu" },
                        { "label6", "Please specify the full path to online.exe. " },
                        { "label8", "You can also set up an auto-get button or drag-and-drop."},
                        { "label9", "" },

                        { "tabPage2", "Chat" },
                        { "checkBox1", "Press the spacebar and then chat." },
                        { "radioButton13", "Send text to PSOBB using MemorySharp (Recommend)" },
                        { "radioButton6", "Send text using only AHK" },
                        { "label11", "I can only send English alphabets, but the problem is less likely to occur" },
                        { "label12", "If you are familiar with AHK script maintenance, you can solve the problem relatively easily." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Default" },
                        { "radioButton7", "AHK is used to control IME, and the character string is sent from ChatLogTool itself." },
                        { "checkBox7", "Use IMEZhToJa.ahk in Chinese mode" },
                        { "label19", "You can send sentences in a mixture of your native Language_GP and English" },
                        { "label15", "space-key Chat delay" },
                        { "label16", "Input mode change waiting time" },
                        { "label17", "Char-to-Char transmission wait" },
                        { "button10", "Default" },
                        { "radioButton8", "Use only the program of ChatLogTool itself to send the character string" },
                        { "label20", "I can\'t send sentences with mixed character codes, but it\'s the fastest" },
                        { "label21", "It is necessary to set the input mode on the PSOBB side each time" },
                        { "label18", "Char-to-Char transmission wait" },
                        { "button11", "Default" },
                        { "radioButton12", "Use Windows PowerShell (.bat) to send strings" },
                        { "label22", "The unit of waiting time is (ms)" },
                        
                        { "tabPage3", "View" },
                        { "radioButton18", "Use ListView for display(Recommend)" },
                        { "radioButton19", "Use ListBox for display" },
                        { "label33", "Character color" },
                        { "label30", "Red (0 to 255)" },
                        { "label31", "Green (0 to 255)" },
                        { "label32", "Blue (0 to 255)" },
                        { "label26", "Back color" },
                        { "label27", "Red (0 to 255)" },
                        { "label28", "Green (0 to 255)" },
                        { "label29", "Blue (0 to 255)" },
                        { "label5", "Font" },
                        { "Button3", "Default" },
                        { "checkBox3", "Float PSOChatLog in the foreground" },
                        { "checkBox4", "Save screen position when exiting" },
                        { "checkBox5", "Save chat when finished" },
                        { "label25", "Number of chat lines to save (Max 9999 lines)" },
                        { "checkBox9", "Omit confirmation message when GoToLobby" },

                        { "tabPage4", "Asynchronous" },
                        { "radioButton16", "Perform translations asynchronously (Recommend)" },
                        { "radioButton17", "Perform translations synchronously" },

                        { "tabPage5", "Translate" },
                        { "radioButton1", "Do not translate" },
                        { "button6", "Copy the Google Apps Scripts(GAS) automatic translation script to the clipboard." },
                        { "button5", "Get the URL of Google Apps Scripts(GAS) (browser opens)" },
                        { "button4", "Get DeepL API key (browser opens)" },
                        { "radioButton5", "I usually use DeepL, and when DeepL returns the \"original text\", I use Google Apps Scripts." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "button12", "Open the GAS configuration manual (browser opens)" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Translate using Google Apps Scripts (GAS)" },
                        { "radioButton3", "Translate using DeepL\'s \"DeepL API Pro\"" },
                        { "radioButton2", "Translate using DeepL\'s \"DeepL API Free\"" },
                        { "checkBox8", "Use Python for DeepL translation" },
                        { "radioButton9", "Use Baidu for translation" },

                        { "tabPage6", "Translate" },
                        { "radioButton14", "Do not translate" },
                        { "radioButton15", "Translate using NICT's TexTra." },
                        { "button14", "Obtain a TexTra API key (browser will open)" },
                        { "button15", "Check TexTra's API key (browser will open)" },

                        //{ "tabPage7", "Slang Dictionary" },

                        { "tabPage8", "Lang Judge" },
                        { "radioButton10", "Priority is given to the JAVA library for automatic Language determination" },
                        { "radioButton11", "Prefer regular expressions for automatic language determination (Recommend)" },
                        { "checkBox6", "Turn on the garbled Cyrillic decoder (Recommend)" },

                        { "tabPage9", "Guild Chat" },
                        { "button8", "Default" },
                        { "button7", "Play" },
                        { "label7", "Volume" },
                        { "checkBox2", "Make a sound when guild chat comes (Recommend)" },
                        { "Text", "configuration" }
                    }
                }
                ,
                {
                    Language_GP.DE,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "OK" },

                        { "tabPage1", "Installieren" },
                        { "label4", "Wo installiere ich Ephinea PSOBB" },
                        { "Button13", "Automatisch aus der Registry abgerufen (empfehlen)" },
                        { "Button2", "Automatisch aus dem Startmenü abgerufen" },
                        { "label6", "Bitte geben Sie den vollständigen Pfad zu online.exe an" },
                        { "label8", "Sie können es auch mit der Schaltfläche für die automatische Erfassung oder per Drag & Drop einstellen" },
                        { "label9", "" },

                        { "tabPage2", "Plaudern" },
                        { "checkBox1", "Drücke die Leertaste und dann chatte" },
                        { "radioButton13", "Senden Sie Text an PSOBB mit MemorySharp (empfehlen)" },

                        { "radioButton6", "Senden Sie Text nur mit AHK" },
                        { "label11", "Ich kann nur englische Alphabete senden, aber das Problem tritt weniger wahrscheinlich auf" },
                        { "label12", "Wenn Sie sich mit der AHK-Skriptpflege auskennen, können Sie das Problem relativ einfach lösen" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Standard" },
                        { "radioButton7", "AHK wird verwendet, um IME zu steuern, und die Zeichenfolge wird von ChatLogTool selbst gesendet" },
                        { "checkBox7", "Verwenden Sie IMEZhToJa.ahk im chinesischen Modus" },
                        { "label19", "Sie können Sätze in einer Mischung aus Ihrer Muttersprache und Englisch senden" },
                        { "label15", "Chatverzögerung mit Leertaste" },
                        { "label16", "Eingabemoduswechsel warten" },
                        { "label17", "Übertragung zwischen Zeichen warten" },
                        { "button10", "Standard" },
                        { "radioButton8", "Verwenden Sie nur das Programm von ChatLogTool selbst, um die Zeichenfolge zu senden" },
                        { "label20", "Sätze mit gemischten Zeichencodes können nicht gesendet werden" },
                        { "label21", "Der Eingabemodus muss jedes Mal auf der PSOBB-Seite eingestellt werden" },
                        { "label18", "Übertragung zwischen Zeichen warten" },
                        { "button11", "Standard" },
                        { "radioButton12", "Verwenden Sie Windows PowerShell (.bat), um Zeichenfolgen zu senden" },
                        { "label22", "Die Einheit der Wartezeit ist (ms)" },

                        { "tabPage3", "Anzeige" },
                        { "radioButton18", "Verwenden Sie ListView für die Anzeige.(empfehlen)" },
                        { "radioButton19", "Verwenden Sie ListBox für die Anzeige." },
                        { "label33", "Buchstabenfarbe" },
                        { "label30", "Rot (0 bis 255)" },
                        { "label31", "Grün (0 bis 255)" },
                        { "label32", "Blau (0 bis 255)" },
                        { "label26", "Hintergrundfarbe" },
                        { "label27", "Rot (0 bis 255)" },
                        { "label28", "Grün (0 bis 255)" },
                        { "label29", "Blau (0 bis 255)" },
                        { "label5", "Schriftart" },
                        { "Button3", "Standard" },
                        { "checkBox3", "PSOChatLog im Vordergrund schweben lassen" },
                        { "checkBox4", "Speichern Sie die Bildschirmposition, wenn Sie fertig sind." },
                        { "checkBox5", "Chat speichern, wenn fertig" },
                        { "label25", "Anzahl der zu speichernden Chatzeilen (max. 9999 Zeilen)" },
                        { "checkBox9", "Lassen Sie die Bestätigungsmeldung weg, wenn GoToLobby" },

                        { "tabPage4", "asynchrone Ausführung" },
                        { "radioButton16", "Übersetzung asynchron ausführen. (empfehlen)" },
                        { "radioButton17", "Synchron laufende Übersetzungen." },

                        { "tabPage5", "Übersetzung" },
                        { "button6", "Kopieren Sie das automatische GAS-Übersetzungsskript in die Zwischenablage" },
                        { "button5", "Rufen Sie die URL von GAS ab (Browser wird geöffnet)" },
                        { "button4", "Holen Sie sich den DeepL-API-Schlüssel (Browser wird geöffnet)" },
                        { "radioButton5", "Normalerweise verwende ich DeepL," },
                        { "label10", "Google Apps-Skripts verwenden, wenn DeepL \"original\" zurückgibt" },
                        { "label3", "URL" },
                        { "button12", "Öffnen Sie das GAS-Konfigurationshandbuch (Browser öffnet sich)" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Mit Google Apps Scripts (GAS) übersetzen" },
                        { "radioButton3", "Übersetzen Sie mit DeepLs \"DeepL API Pro\"" },
                        { "radioButton2", "Übersetzen Sie mit DeepLs \"DeepL API Free\"" },
                        { "checkBox8", "Verwenden Sie Python für die DeepL-Übersetzung" },
                        { "radioButton1", "Übersetze nicht" },
                        { "radioButton9", "Verwenden Sie Baidu für die Übersetzung" },

                        { "tabPage6", "Übersetzung" },
                        { "radioButton14", "bersetze nicht" },
                        { "radioButton15", "Übersetzen Sie mit NICTs TexTra." },
                        { "button14", "Besorgen Sie sich einen TexTra-API-Schlüssel (Browser wird geöffnet)." },
                        { "button15", "Prüfen Sie den API-Schlüssel für TexTra (Browser wird geöffnet)." },

                        //{ "tabPage7", "SLANG-Wörterbuch\r\n" },

                        { "tabPage8", "Lang Richter" },
                        { "radioButton10", "Priorität hat die JAVA-Bibliothek für die automatische Sprachbestimmung" },
                        { "radioButton11", "Priorisieren von regulären Ausdrücken für die automatische Sprachbestimmung (empfehlen)" },
                        { "checkBox6", "Schalte den verstümmelten kyrillischen Decoder ein (empfehlen)" },

                        { "tabPage9", "Gildenchat" },
                        { "button8", "Standard" },
                        { "button7", "Reproduktion" },
                        { "label7", "Volumen" },
                        { "checkBox2", "Mach einen Ton, wenn der Gildenchat kommt (empfehlen)" },
                        { "Text", "Konfiguration" }
                    }
                }
                ,
                {
                    Language_GP.ES,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "OK" },

                        { "tabPage1", "Instalar" },
                        { "label4", "Dónde instalar Ephinea PSOBB" },
                        { "Button13", "Se obtiene automáticamente del registro (recomendación)" },
                        { "Button2", "Se obtiene automáticamente del menú Inicio" },
                        { "label6", "Especifique con la ruta completa a online.exe" },
                        { "label8", "También puede configurarlo con el botón de adquisición automática o arrastrar y soltar" },
                        { "label9", "" },

                        { "tabPage2", "chat" },
                        { "checkBox1", "Presiona la barra espaciadora y luego chatea" },
                        { "radioButton13", "Enviar texto a PSOBB usando MemorySharp (recomendación)" },
                        { "radioButton6", "Envía mensajes de texto usando solo AHK" },
                        { "label11", "Solo puedo enviar alfabetos en inglés, pero es menos probable que ocurra el problema" },
                        { "label12", "Si está familiarizado con el mantenimiento del script AHK, puede resolver el problema con relativa facilidad." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Defecto" },
                        { "radioButton7", "AHK se utiliza para controlar IME y la cadena de caracteres se envía desde ChatLogTool." },
                        { "checkBox7", "Use IMEZhToJa.ahk en modo chino" },
                        { "label19", "Puede enviar oraciones en una mezcla de su idioma nativo e inglés." },
                        { "label15", "space-key Chat delay" },
                        { "label16", "Espera de cambio de modo de entrada" },
                        { "label17", "Espera de transmisión entre caracteres" },
                        { "button10", "Defecto" },
                        { "radioButton8", "Utilice solo el programa de ChatLogTool para enviar la cadena de caracteres" },
                        { "label20", "No se pueden enviar frases con códigos de caracteres mixtos" },
                        { "label21", "Es necesario configurar el modo de entrada en el lado PSOBB cada vez." },
                        { "label18", "Espera de transmisión entre caracteres" },
                        { "button11", "Defecto" },
                        { "radioButton12", "Use Windows PowerShell (.bat) para enviar cadenas" },
                        { "label22", "La unidad de tiempo de espera es (ms)." },

                        { "tabPage3", "monitor" },
                        { "radioButton18", "Utilice ListView para la visualización.(recomendación)" },
                        { "radioButton19", "Utilice ListBox para la visualización." },
                        { "label33", "color de la letra" },
                        { "label30", "Rojo (0 a 255)" },
                        { "label31", "Verde (0 a 255)" },
                        { "label32", "Azul (0 a 255)" },
                        { "label26", "color de fondo" },
                        { "label27", "Rojo (0 a 255)" },
                        { "label28", "Verde (0 a 255)" },
                        { "label29", "Azul (0 a 255)" },
                        { "label5", "fuente" },
                        { "Button3", "Defecto" },
                        { "checkBox3", "Flotar PSOChatLog en primer plano" },
                        { "checkBox4", "Guardar la posición de la pantalla al salir" },
                        { "checkBox5", "Guardar chat cuando termine" },
                        { "label25", "Número de líneas de chat para guardar (máximo 9999 líneas)" },
                        { "checkBox9", "Omita el mensaje de confirmación cuando GoToLobby" },

                        { "tabPage4", "ejecución asíncrona" },
                        { "radioButton16", "Ejecutar la traducción de forma asíncrona. (recomendación)" },
                        { "radioButton17", "Ejecución sincrónica de las traducciones." },

                        { "tabPage5", "traducción" },
                        { "button6", "Copiar el script de traducción automática de Google Apps Scripts(GAS) al portapapeles" },
                        { "button5", "Obtener la URL de Google Apps Scripts(GAS) (se abre el navegador)" },
                        { "button4", "Obtener la clave de API de DeepL (se abre el navegador)" },
                        { "radioButton5", "Normalmente uso DeepL, y cuando DeepL devuelve el \"texto original\", utilizo Google Apps Scripts." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "button12", "Abra el manual de configuración de GAS (se abre el navegador)" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Traducir usando Google Apps Scripts (GAS)" },
                        { "radioButton3", "Traducir usando \"DeepL API Pro\" de DeepL" },
                        { "radioButton2", "Traduce usando \"DeepL API Free\" de DeepL" },
                        { "checkBox8", "Utilice Python para la traducción DeepL" },
                        { "radioButton1", "No traducir" },
                        { "radioButton9", "Utilice Baidu para la traducción" },

                        { "tabPage6", "traducción" },
                        { "radioButton14", "No traducir" },
                        { "radioButton15", "Traduce con TexTra de NICT." },
                        { "button14", "Obtenga una clave API TexTra (se abrirá el navegador)." },
                        { "button15", "Compruebe la clave API de TexTra (se abrirá el navegador)." },

                        //{ "tabPage7", "Diccionario SLANG" },

                        { "tabPage8", "Juez de Lang" },
                        { "radioButton10", "Se da prioridad a la biblioteca JAVA para la determinación automática del idioma" },
                        { "radioButton11", "Priorice las expresiones regulares para la determinación automática del idioma (recomendación)" },
                        { "checkBox6", "Encienda el decodificador cirílico ilegible (recomendación)" },

                        { "tabPage9", "Chat del gremio" },
                        { "button8", "Defecto" },
                        { "button7", "reproducción" },
                        { "label7", "volumen" },
                        { "checkBox2", "Haz un sonido cuando llegue el chat del gremio (recomendación)" },
                        { "Text", "Configuración" }
                    }
                }
                ,
                {
                    Language_GP.FR,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "OK" },

                        { "tabPage1", "installer" },
                        { "label4", "Où installer Ephinea PSOBB" },
                        { "Button13", "Obtenu automatiquement à partir du registre (recommandé)" },
                        { "Button2", "Récupération automatique à partir du menu Démarrer" },
                        { "label6", "Veuillez spécifier avec le chemin complet vers online.exe" },
                        { "label8", "Vous pouvez également le paramétrer avec le bouton d'acquisition automatique ou par glisser-déposer" },
                        { "label9", "" },

                        { "tabPage2", "discuter" },
                        { "checkBox1", "Appuyez sur la barre d'espace puis discutez" },
                        { "radioButton13", "Envoyer du texte à PSOBB à l'aide de MemorySharp (recommandé)" },
                        { "radioButton6", "Envoyer du texte en utilisant uniquement AHK" },
                        { "label11", "Je ne peux envoyer que des alphabets anglais, mais le problème est moins susceptible de se produire" },
                        { "label12", "Si vous êtes familiarisé avec la maintenance des scripts AHK, vous pouvez résoudre le problème assez facilement." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Défaut" },
                        { "radioButton7", "AHK est utilisé pour contrôler l'IME et la chaîne de caractères est envoyée depuis ChatLogTool lui-même." },
                        { "checkBox7", "Utilisez IMEZhToJa.ahk en mode chinois" },
                        { "label19", "Vous pouvez envoyer des phrases dans un mélange de votre langue maternelle et de l'anglais" },
                        { "label15", "délai de discussion avec la touche espace" },
                        { "label16", "Attente de changement de mode d'entrée" },
                        { "label17", "Attente de transmission inter-caractères" },
                        { "button10", "Défaut" },
                        { "radioButton8", "Utilisez uniquement le programme de ChatLogTool lui-même pour envoyer la chaîne de caractères" },
                        { "label20", "Impossible d'envoyer des phrases avec des codes de caractères mixtes" },
                        { "label21", "Il est nécessaire de régler à chaque fois le mode d'entrée côté PSOBB." },
                        { "label18", "Attente de transmission inter-caractères" },
                        { "button11", "Défaut" },
                        { "radioButton12", "Utiliser Windows PowerShell (.bat) pour envoyer des chaînes" },
                        { "label22", "L'unité de temps d'attente est (ms)." },

                        { "tabPage3", "affichage" },
                        { "radioButton18", "Utiliser ListView pour l'affichage.(recommandé)" },
                        { "radioButton19", "Utiliser ListBox pour l'affichage." },
                        { "label33", "Couleur de la lettre" },
                        { "label30", "Rouge (0 à 255)" },
                        { "label31", "Vert (0 à 255)" },
                        { "label32", "Bleu (0 à 255)" },
                        { "label26", "couleur de fond" },
                        { "label27", "Rouge (0 à 255)" },
                        { "label28", "Vert (0 à 255)" },
                        { "label29", "Bleu (0 à 255)" },
                        { "label5", "Font" },
                        { "Button3", "Défaut" },
                        { "checkBox3", "Flotteur PSOCatLog au premier plan" },
                        { "checkBox4", "Enregistrer la position de l'écran en quittant" },
                        { "checkBox5", "Enregistrer le chat lorsque vous avez terminé" },
                        { "label25", "Nombre de lignes de chat à enregistrer (Max 9999 lignes)" },
                        { "checkBox9", "Omettre le message de confirmation lorsque GoToLobby" },

                        { "tabPage4", "exécution asynchrone" },
                        { "radioButton16", "Exécuter la traduction de manière asynchrone. (recommandé)" },
                        { "radioButton17", "Exécution synchrone des traductions." },

                        { "tabPage5", "Traduction" },
                        { "button6", "Copier le script de traduction automatique Google Apps Scripts(GAS) dans le presse-papiers" },
                        { "button5", "Obtenez l\'URL de Google Apps Scripts(GAS) (le navigateur s\'ouvre)" },
                        { "button4", "Obtenir la clé API DeepL (le navigateur s\'ouvre)" },
                        { "radioButton5", "J'utilise généralement DeepL, et lorsque DeepL renvoie le \"texte d\'origine\", j\'utilise les scripts Google Apps." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "button12", "Ouvrir le manuel de configuration GAS (le navigateur s'ouvre)" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Traduire à l\'aide de Google Apps Scripts (GAS)" },
                        { "radioButton3", "Traduire à l\'aide de \"DeepL API Pro\" de DeepL" },
                        { "radioButton2", "Traduire en utilisant \"DeepL API Free\" de DeepL" },
                        { "checkBox8", "Utiliser Python pour la traduction DeepL" },
                        { "radioButton1", "Ne traduisez pas" },
                        { "radioButton9", "Utiliser Baidu pour la traduction" },

                        { "tabPage6", "Traduction" },
                        { "radioButton14", "Ne traduisez pas" },
                        { "radioButton15", "Traduire à l'aide de TexTra de NICT." },
                        { "button14", "Obtenir une clé API TexTra (le navigateur s'ouvre)" },
                        { "button15", "Vérifiez la clé API de TexTra (le navigateur s'ouvrira)." },

                        //{ "tabPage7", "SLANG dictionnaire" },

                        { "tabPage8", "Juge Lang" },
                        { "radioButton10", "La priorité est donnée à la bibliothèque JAVA pour la détermination automatique de la langue" },
                        { "radioButton11", "Donner la priorité aux expressions régulières pour la détermination automatique de la langue (recommandé)" },
                        { "checkBox6", "Allumez le décodeur cyrillique brouillé (recommandé)" },

                        { "tabPage9", "Chat de guilde" },
                        { "button8", "Défaut" },
                        { "button7", "la reproduction" },
                        { "label7", "le volume" },
                        { "checkBox2", "Faire un son lorsque le chat de guilde arrive (recommandé)" },
                        { "Text", "Configuration" }
                    }
                }
                ,
                {
                    Language_GP.RU,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "ОК" },

                        { "tabPage1", "установить" },
                        { "label4", "Где установить Ephinea PSOBB" },
                        { "Button13", "Получается автоматически из реестра (рекомендуется)" },
                        { "Button2", "Автоматически извлекается из меню \"Пуск\"" },
                        { "label6", "Укажите полный путь к online.exe" },
                        { "label8", "Кнопка автоматического захвата или" },
                        { "label9", "Вы также можете установить перетаскиванием" },

                        { "tabPage2", "чат" },
                        { "checkBox1", "Нажмите пробел, а затем поговорите" },
                        { "radioButton13", "Отправить текст в PSOBB с помощью MemorySharp (рекомендуется)" },
                        { "radioButton6", "Отправить текст, используя только AHK" },
                        { "label11", "Могут быть отправлены только английские алфавиты" },
                        { "label12", "Если вы посмотрите на сценарий AHK, вы можете решить проблему относительно легко." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Дефолт" },
                        { "radioButton7", "AHK используется для управления IME, а строка символов отправляется из самого ChatLogTool." },
                        { "checkBox7", "Используйте IMEZhToJa.ahk в китайском режиме" },
                        { "label19", "Вы можете отправлять смешанные предложения на родном и английском языках." },
                        { "label15", "space-key Chat delay" },
                        { "label16", "Input mode change waiting time" },
                        { "label17", "Char-to-Char transmission wait" },
                        { "button10", "Дефолт" },
                        { "radioButton8", "Используйте только программу самого ChatLogTool для отправки символьной строки" },
                        { "label20", "Невозможно отправлять предложения со смешанными кодами символов" },
                        { "label21", "Необходимо каждый раз устанавливать режим ввода на стороне PSOBB." },
                        { "label18", "Char-to-Char transmission wait" },
                        { "button11", "Дефолт" },
                        { "radioButton12", "Используйте Windows PowerShell (.bat) для отправки строк" },
                        { "label22", "Единица времени ожидания - (мс)." },

                        { "tabPage3", "отображать" },
                        { "radioButton18", "Используйте ListView для отображения.(рекомендуется)" },
                        { "radioButton19", "Используйте ListBox для отображения." },
                        { "label33", "Цвет букв" },
                        { "label30", "Красный (от 0 до 255)" },
                        { "label31", "Зеленый (от 0 до 255)" },
                        { "label32", "Синий (от 0 до 255)" },
                        { "label26", "цвет фона" },
                        { "label27", "Красный (от 0 до 255)" },
                        { "label28", "Зеленый (от 0 до 255)" },
                        { "label29", "Синий (от 0 до 255)" },
                        { "label5", "Шрифт" },
                        { "Button3", "Дефолт" },
                        { "checkBox3", "Плавающий PSOChatLog на переднем плане" },
                        { "checkBox4", "Сохранить положение экрана при выходе" },
                        { "checkBox5", "Сохраните чат, когда закончите" },
                        { "label25", "Количество сохраняемых строк чата (максимум 9999 строк)" },
                        { "checkBox9", "Опустите сообщение о подтверждении, когда GoToLobby" },

                        { "tabPage4", "асинхронный" },
                        { "radioButton16", "Перевод выполняется не Синхронное выполнение (рекомендуется)" },
                        { "radioButton17", "Выполнение переводов синхронно." },

                        { "tabPage5", "перевод" },
                        { "button6", "Скопировать скрипт автоматического перевода GAS в буфер обмена" },
                        { "button5", "Получить URL-адрес Google Apps Scripts(GAS) (открывается браузер)" },
                        { "button4", "Получить ключ DeepL API (открывается браузер)" },
                        { "radioButton5", "Я обычно использую DeepL," },
                        { "label10", "Используйте скрипты Google Apps, когда DeepL возвращает \"оригинал\"" },
                        { "label3", "URL" },
                        { "button12", "Откройте руководство по настройке GAS (открывается браузер)" },
                        { "label2", "КЛЮЧ API" },
                        { "label1", "КЛЮЧ API" },
                        { "radioButton4", "Перевести с помощью скриптов Google Apps (GAS)" },
                        { "radioButton3", "Переводите с помощью DeepL API Pro" },
                        { "radioButton2", "Переводите с помощью DeepL API Free DeepL" },
                        { "checkBox8", "Используйте Python для перевода DeepL" },
                        { "radioButton1", "Не переводить" },
                        { "radioButton9", "Используйте Baidu для перевода" },

                        { "tabPage6", "перевод" },
                        { "radioButton14", "Не переводить" },
                        { "radioButton15", "Переводите с помощью TexTra от NICT" },
                        { "button14", "Получите ключ API TexTra (откроется браузер)" },
                        { "button15", "Проверьте API-ключ для TexTra (откроется браузер)." },
                        
                        //{ "tabPage7", "Словарь SLANG" },

                        { "tabPage8", "Ланг судья" },
                        { "radioButton10", "Приоритет отдается библиотеке JAVA для автоматического определения языка" },
                        { "radioButton11", "Предпочитайте регулярные выражения для автоматического определения языка (рекомендуется)" },
                        { "checkBox6", "Включите неверный декодер кириллицы (рекомендуется)" },

                        { "tabPage9", "Чат гильдии" },
                        { "button8", "Дефолт" },
                        { "button7", "воспроизведение" },
                        { "label7", "объем" },
                        { "checkBox2", "Сделайте звук, когда появится чат гильдии (рекомендуется)" },
                        { "Text", "Конфигурация" }
                    }
                }
                ,
                {
                    Language_GP.ZH_CN,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "确定" },

                        { "tabPage1", "安装" },
                        { "label4", "在哪里安装 Ephinea PSOBB" },
                        { "Button13", "从注册表中自动获取（推荐）" },
                        { "Button2", "从开始菜单中自动检索" },
                        { "label6", "请使用 online.exe 的完整路径指定" },
                        { "label8", "也可以通过自动获取按钮或拖放设置" },
                        { "label9", "" },

                        { "tabPage2", "聊天" },
                        { "checkBox1", "按空格键然后聊天" },
                        { "radioButton13", "使用 MemorySharp 向 PSOBB 发送文本（推荐）" },
                        { "radioButton6", "仅使用 AHK 发送文本" },
                        { "label11", "我只能发送英文字母，但出现问题的可能性较小" },
                        { "label12", "如果您熟悉AHK脚本维护，则可以相对轻松地解决问题" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "默认" },
                        { "radioButton7", "AHK用于控制输入法，字符串由ChatLogTool自身发送" },
                        { "checkBox7", "在中文模式下使用 IMEZhToJa.ahk" },
                        { "label19", "您可以混合使用您的母语和英语发送句子" },
                        { "label15", "空格键聊天延迟" },
                        { "label16", "输入模式改变等待" },
                        { "label17", "字符间传输等待" },
                        { "button10", "默认" },
                        { "radioButton8", "仅使用 ChatLogTool 本身的程序发送字符串" },
                        { "label20", "不能发送混合字符代码的句子，包括全角和半角字符" },
                        { "label21", "每次都需要在PSOBB端设置输入模式" },
                        { "label18", "字符间传输等待" },
                        { "button11", "默认" },
                        { "radioButton12", "使用 Windows PowerShell (.bat) 发送字符串" },
                        { "label22", "等待时间的单位是（ms）" },

                        { "tabPage3", "展示" },
                        { "radioButton18", "使用ListView进行显示。(推荐)" },
                        { "radioButton19", "使用ListBox进行显示。" },
                        { "label33", "字母顏色" },
                        { "label30", "红色（0至255）" },
                        { "label31", "绿色（0至255）" },
                        { "label32", "蓝色（0至255）" },
                        { "label26", "背景颜色" },
                        { "label27", "红色（0至255）" },
                        { "label28", "绿色（0至255）" },
                        { "label29", "蓝色（0至255）" },
                        { "label5", "字体" },
                        { "Button3", "默认" },
                        { "checkBox3", "在前台浮动 PSOChatLog" },
                        { "checkBox4", "退出时保存屏幕位置" },
                        { "checkBox5", "完成后保存聊​​天" },
                        { "label25", "要保存的聊天行数（最多 9999 行）" },
                        { "checkBox9", "省略GoToLobby时的确认信息" },

                        { "tabPage4", "非同步的" },
                        { "radioButton16", "异步地运行翻译。（推荐）" },
                        { "radioButton17", "同步运行翻译。" },

                        { "tabPage5", "翻译" },
                        { "button6", "将 Google Apps Scripts(GAS) 自动翻译脚本复制到剪贴板" },
                        { "button5", "获取 Google Apps Scripts(GAS) 的 URL（浏览器打开）" },
                        { "button4", "获取 DeepL API 密钥（浏览器打开）" },
                        { "radioButton5", "我通常使用 DeepL，当 DeepL 返回“原始文本”时，我使用 Google Apps Scripts" },
                        { "label10", "" },
                        { "label3", "网址" },
                        { "button12", "打开 GAS 配置手册（浏览器打开）" },
                        { "label2", "接口密钥" },
                        { "label1", "接口密钥" },
                        { "radioButton4", "使用 Google Apps 脚本 (GAS) 进行翻译" },
                        { "radioButton3", "使用 DeepL 的“DeepL API Pro”进行翻译" },
                        { "radioButton2", "使用 DeepL 的“DeepL API Free”进行翻译" },
                        { "checkBox8", "使用 Python 进行 DeepL 翻译" },
                        { "radioButton1", "不翻译" },
                        { "radioButton9", "使用百度翻译" },

                        { "tabPage6", "翻译" },
                        { "radioButton14", "不翻译" },
                        { "radioButton15", "使用NICT的TexTra进行翻译。" },
                        { "button14", "获得TexTra API密钥（浏览器将打开）。" },
                        { "button15", "检查TexTra的API密钥（浏览器将打开）。" },

                        //{ "tabPage7", "俚语词典" },

                        { "tabPage8", "郎法官" },
                        { "radioButton10", "优先使用JAVA库进行自动语言判断" },
                        { "radioButton11", "为自动语言确定优先考虑正则表达式（推荐）" },
                        { "checkBox6", "打开乱码的西里尔文解码器（推荐）" },

                        { "tabPage9", "公会聊天" },
                        { "button8", "默认" },
                        { "button7", "再生产" },
                        { "label7", "体积" },
                        { "checkBox2", "公会聊天来时发出声音（推荐）" },
                        { "Text", "配置" }
                    }
                }
                ,
                {
                    Language_GP.ZH_TW,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "確定" },

                        { "tabPage1", "安裝" },
                        { "label4", "在哪里安裝 Ephinea PSOBB" },
                        { "Button13", "從註冊表自動獲取（推薦）" },
                        { "Button2", "從開始菜單自動獲取" },
                        { "label6", "請使用 online.exe 的完整路徑指定" },
                        { "label8", "也可以通過自動採集按鈕或拖拽設置" },
                        { "label9", "" },

                        { "tabPage2", "聊天" },
                        { "checkBox1", "按空格鍵然後聊天" },
                        { "radioButton13", "使用 MemorySharp 向 PSOBB 發送文本（推薦）" },
                        { "radioButton6", "僅使用 AHK 發送文本" },
                        { "label11", "我只能發送英文字母，但出現問題的可能性較小" },
                        { "label12", "如果您熟悉AHK腳本維護，則可以相對輕鬆地解決問題" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "默認" },
                        { "radioButton7", "AHK用於控制輸入法，字符串由ChatLogTool自身發送" },
                        { "checkBox7", "在中文模式下使用 IMEZhToJa.ahk" },
                        { "label19", "您可以混合使用您的母語和英語發送句子" },
                        { "label15", "空格鍵聊天延遲" },
                        { "label16", "輸入模式改變等待" },
                        { "label17", "字符間傳輸等待" },
                        { "button10", "默認" },
                        { "radioButton8", "僅使用 ChatLogTool 本身的程序發送字符串" },
                        { "label20", "不能發送混合字符代碼的句子，包括全角和半角字符" },
                        { "label21", "每次都需要在PSOBB端設置輸入模式" },
                        { "label18", "字符間傳輸等待" },
                        { "button11", "默認" },
                        { "radioButton12", "使用 Windows PowerShell (.bat) 發送字符串" },
                        { "label22", "等待時間的單位是（ms）" },

                        { "tabPage3", "展示" },
                        { "radioButton18", "使用ListView进行显示。(推荐)" },
                        { "radioButton19", "使用ListBox进行显示。" },
                        { "label33", "字母颜色" },
                        { "label30", "红色（0至255）" },
                        { "label31", "绿色（0至255）" },
                        { "label32", "蓝色（0至255）" },
                        { "label26", "背景颜色" },
                        { "label27", "红色（0至255）" },
                        { "label28", "绿色（0至255）" },
                        { "label29", "蓝色（0至255）" },
                        { "label5", "字體" },
                        { "Button3", "默認" },
                        { "checkBox3", "在前台浮動 PSOChatLog" },
                        { "checkBox4", "退出時保存屏幕位置" },
                        { "checkBox5", "完成後保存聊天" },
                        { "label25", "要保存的聊天行數（最多 9999 行）" },
                        { "checkBox9", "省略GoToLobby时的确认信息" },

                        { "tabPage4", "非同步的" },
                        { "radioButton16", "异步地运行翻译。（推荐）" },
                        { "radioButton17", "同步运行翻译。" },

                        { "tabPage5", "翻譯" },
                        { "button6", "將 Google Apps Scripts(GAS) 自動翻譯腳本複製到剪貼板" },
                        { "button5", "獲取 Google Apps Scripts(GAS) 的 URL（瀏覽器打開）" },
                        { "button4", "獲取 DeepL API 密鑰（瀏覽器打開）" },
                        { "radioButton5", "我通常使用 DeepL，當 DeepL 返回“原始文本”時，我使用 Google Apps Scripts" },
                        { "label10", "" },
                        { "label3", "網址" },
                        { "button12", "打開 GAS 配置手冊（瀏覽器打開）" },
                        { "label2", "接口密鑰" },
                        { "label1", "接口密鑰" },
                        { "radioButton4", "使用 Google Apps 腳本 (GAS) 進行翻譯" },
                        { "radioButton3", "使用 DeepL 的“DeepL API Pro”進行翻譯" },
                        { "radioButton2", "使用 DeepL 的“DeepL API Free”進行翻譯" },
                        { "checkBox8", "使用 Python 進行 DeepL 翻譯" },
                        { "radioButton1", "不翻譯" },
                        { "radioButton9", "使用百度翻譯" },

                        { "tabPage6", "翻譯" },
                        { "radioButton14", "不翻譯" },
                        { "radioButton15", "使用NICT的TexTra进行翻译。" },
                        { "button14", "获得TexTra API密钥（浏览器将打开）。" },
                        { "button15", "检查TexTra的API密钥（浏览器将打开）。" },

                        //{ "tabPage7", "俚语词典" },

                        { "tabPage8", "郎法官" },
                        { "radioButton10", "優先使用JAVA庫進行自動語言判斷" },
                        { "radioButton11", "為自動語言確定優先考慮正則表達式（推薦）" },
                        { "checkBox6", "打開亂碼的西里爾文解碼器（推薦）" },

                        { "tabPage9", "公會聊天" },
                        { "button8", "默認" },
                        { "button7", "再生產" },
                        { "label7", "體積" },
                        { "checkBox2", "公會聊天來時發出聲音（推薦）" },
                        { "Text", "配置" }
                    }
                }
                ,
                {
                    Language_GP.JA,
                    new Dictionary<string, string>()
                    {
                        { "Text", "設定" },
                        { "tabPage1", "インストール" },
                        { "label4", "Ephinea PSOBBのインストール先" },
                        { "Button13", "レジストリから自動取得（おすすめ）" },
                        { "Button2", "スタートメニューから自動取得" },
                        { "label6", "online.exeまでのフルパスで指定してください" },
                        { "label8", "自動取得ボタンか、ドラッグアンドドロップでも設定できます" },
                        { "label9", "" },

                        { "tabPage2", "チャット" },
                        { "checkBox1", "スペースキーを押してからチャットをする" },
                        { "radioButton13", "MemorySharpを利用してPSOBBにテキストを送信する（おすすめ）" },
                        { "radioButton6", "AHKのみを利用して、テキスト送信する" },
                        { "label11", "英語のアルファベットしか送信できませんが、問題は発生しづらいです" },
                        { "label12", "詳しい方がAHKスクリプトを保守すれば、割と簡単に問題解決も図れます" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "デフォルト" },
                        { "radioButton7", "IMEのコントロールにAHKを利用して、文字列はChatLogTool本体から送信する" },
                        { "checkBox7", "中国語モードでIMEZhToJa.ahkを利用する" },
                        { "label19", "日本語と英語の混ざった文章を送信できます" },
                        { "label15", "スペースを押してからチャットを開始するまでの時間" },
                        { "label16", "入力モード変更待ち時間" },
                        { "label17", "文字間送信待ち時間" },
                        { "button10", "デフォルト" },
                        { "radioButton8", "文字列の送信にChatLogTool本体のプログラムのみを利用する" },
                        { "label20", "全角半角を含め、文字コードの混ざった文章を送信できませんが、最も高速です" },
                        { "label21", "つどPSOBB側で、入力モードを設定する必要があります" },
                        { "label18", "文字間送信待ち時間" },
                        { "button11", "デフォルト" },
                        { "radioButton12", "文字列の送信にWindows PowerShell (.bat)を利用する" },
                        { "label22", "待ち時間(Wait)は、全て単位が(ms)です" },

                        { "tabPage3", "表示" },
                        { "radioButton18", "表示にListViewを使用する。(おすすめ)" },
                        { "radioButton19", "表示にListBoxを使用する。" },
                        { "label33", "文字色" },
                        { "label30", "赤 (0 から 255)" },
                        { "label31", "緑 (0 から 255)" },
                        { "label32", "青 (0 から 255)" },
                        { "label26", "背景色" },
                        { "label27", "赤 (0 から 255)" },
                        { "label28", "緑 (0 から 255)" },
                        { "label29", "青 (0 から 255)" },
                        { "label5", "Font" },
                        { "Button3", "デフォルト" },
                        { "checkBox3", "PSOChatLogを最前面にフローティングする" },
                        { "checkBox4", "終了時に画面の位置を保存する" },
                        { "checkBox5", "終了時にチャットを保存する" },
                        { "label25", "保存するチャットの行数(Max9999行)" },
                        { "checkBox9", "GoToLobby時に確認メッセージを省略する" },

                        { "tabPage4", "非同期実行" },
                        { "radioButton16", "翻訳を非同期で実行する（おすすめ）" },
                        { "radioButton17", "翻訳を同期で実行する" },

                        { "tabPage5", "翻訳" },
                        { "radioButton1", "翻訳をしない" },
                        { "radioButton2", "DeepLの「DeepL API Free」を利用して翻訳する" },
                        { "checkBox8", "DeepLの翻訳にPythonを利用する" },
                        { "label1", "API KEY" },
                        { "button4", "DeepLのAPIキーを取得する(ブラウザが開きます)" },
                        { "radioButton3", "DeepLの「DeepL API Pro」を利用して翻訳する" },
                        { "label2", "API KEY" },
                        { "radioButton4", "Google Apps Scripts(GAS)を利用して翻訳する" },
                        { "label3", "URL" },
                        { "button12", "GASの設定マニュアルを開く(ブラウザが開きます)" },
                        { "button6", "GASの自動翻訳スクリプトをクリップボードにコピーする" },
                        { "button5", "GASのURLを取得する(ブラウザが開きます)" },
                        { "radioButton5", "普段はDeepLを利用し、DeepLが「原文」を返した時にGoogle Apps Scriptsを利用する" },
                        { "label10", "" },
                        { "radioButton9", "翻訳にBaiduを利用する" },

                        { "tabPage6", "翻訳" },
                        { "radioButton14", "翻訳をしない" },
                        { "radioButton15", "NICTの「TexTra」を利用して翻訳する" },
                        { "button14", "TexTraのAPIキーを取得する(ブラウザが開きます)" },
                        { "button15", "TexTraのAPIキーを確認する(ブラウザが開きます)"},

                        //{ "tabPage7", "スラング辞書" },

                        { "tabPage8", "言語の自動判断" },
                        { "radioButton10", "言語の自動判断に、JAVAライブラリを優先する" },
                        { "radioButton11", "言語の自動判断に、正規表現を優先する（おすすめ）" },
                        { "checkBox6", "文字化けしたキリル文字のデコーダーをオンにする（おすすめ）" },

                        { "tabPage9", "ギルドチャット" },
                        { "button8", "デフォルト" },
                        { "button7", "再生" },
                        { "label7", "ボリューム" },
                        { "checkBox2", "ギルドチャットが来たら音を鳴らす（おすすめ）" },
                        { "Button1", "OK" }
                    }
                }
                ,
                {
                    Language_GP.KO,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "확인" },
                        { "tabPage1", "설치" },
                        { "label4", "Ephinea PSOBB가 설치된 위치" },
                        { "Button13", "레지스트리에서 자동 취득 (추천)" },
                        { "Button2", "시작 메뉴에서 자동 획득" },
                        { "label6", "online.exe까지의 전체 경로로 지정하십시오" },
                        { "label8", "자동 획득 버튼이나 드래그 앤 드롭으로 설정할 수 있습니다" },
                        { "label9", "" },

                        { "tabPage2", "채팅" },
                        { "checkBox1", "스페이스바를 누른 후 채팅하기" },
                        { "radioButton13", "MemorySharp를 사용하여 PSOBB에 텍스트 보내기 (추천)" },
                        { "radioButton6", "AHK만 사용하여 텍스트 전송" },
                        { "label11", "영어 알파벳만 보낼 수 있지만 문제가 발생하기 어렵습니다" },
                        { "label12", "자세한 쪽이 AHK 스크립트를 보수하면, 비교적 간단하게 문제 해결도 도모할 수 있습니다" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "기본" },
                        { "radioButton7", "IME 컨트롤에 AHK를 사용하여 문자열은 ChatLogTool 본문에서 전송" },
                        { "checkBox7", "중국어 모드에서 IMEZhToJa.ahk 사용" },
                        { "label19", "모국어와 영어가 섞인 문장을 보낼 수 있습니다" },
                        { "label15", "space-key Chat delay" },
                        { "label16", "입력 모드 변경 wait" },
                        { "label17", "문자 간 전송 wait" },
                        { "button10", "기본" },
                        { "radioButton8", "문자열 전송에 ChatLogTool 본문 프로그램만 사용" },
                        { "label20", "전각 반각을 포함하여 문자 코드가 섞인 문장을 보낼 수 없습니다" },
                        { "label21", "언제나 PSOBB 측에서 입력 모드를 설정해야합니다" },
                        { "label18", "문자 간 전송 wait" },
                        { "button11", "기본" },
                        { "radioButton12", "문자열 전송에 Windows PowerShell(.bat) 사용" },
                        { "label22", "대기 시간은 모두 단위가 (ms)입니다" },

                        { "tabPage3", "표시" },
                        { "radioButton18", "표시를 위해 ListView를 사용한다.(추천)" },
                        { "radioButton19", "표시를 위해 ListBox를 사용한다." },
                        { "label33", "문자색" },
                        { "label30", "빨강 (0 to 255)" },
                        { "label31", "녹색 (0 to 255)" },
                        { "label32", "블루 (0 to 255)" },
                        { "label26", "배경색" },
                        { "label27", "빨강 (0 to 255)" },
                        { "label28", "녹색 (0 to 255)" },
                        { "label29", "블루 (0 to 255)" },
                        { "label5", "Font" },
                        { "Button3", "기본" },
                        { "checkBox3", "PSOChatLog를 맨 앞으로 부동" },
                        { "checkBox4", "종료 시 화면 위치 저장" },
                        { "checkBox5", "종료 시 채팅 저장" },
                        { "label25", "저장할 채팅 행 수 (Max9999 행)" },
                        { "checkBox9", "GoToLobby시 확인 메시지 생략" },

                        { "tabPage4", "비동기" },
                        { "radioButton16", "번역을 비동기식으로 실행하기 (추천)" },
                        { "radioButton17", "번역을 동기화하여 실행하기" },

                        { "tabPage5", "번역" },
                        { "button6", "Google Apps Scripts(GAS) 자동 번역 스크립트를 클립보드에 복사" },
                        { "button5", "Google Apps Scripts(GAS) URL 가져 오기 (브라우저가 열립니다)" },
                        { "button4", "DeepL의 API 키 가져 오기 (브라우저가 열립니다) 2021/11/10 현재 한국어를 지원하지 않음" },
                        { "radioButton5", "평소에는 DeepL을 이용하고 DeepL이 '원문'을 반환했을 때 Google Apps Scripts를 이용한다." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "button12", "GAS 설정 매뉴얼 열기 (브라우저가 열립니다)" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Google Apps Scripts (GAS)를 사용하여 번역" },
                        { "radioButton3", "DeepL의 \"DeepL API Pro\"를 이용하여 번역하기" },
                        { "radioButton2", "DeepL의 \"DeepL API Free\"를 이용하여 번역하기" },
                        { "checkBox8", "DeepL 번역에 파이썬 사용" },
                        { "radioButton1", "번역 안함" },
                        { "radioButton9", "번역에 Baidu 사용" },

                        { "tabPage6", "번역" },
                        { "radioButton14", "번역 안함" },
                        { "radioButton15", "NICT의 TexTra를 사용하여 번역하세요." },
                        { "button14", "TexTra API 키 가져오기(브라우저가 열림)" },
                        { "button15", "텍스트라의 API 키 확인(브라우저가 열림)" },

                        //{ "tabPage7", "속어 사전" },

                        { "tabPage8", "랭 판사" },
                        { "radioButton10", "언어 자동 판단에 JAVA 라이브러리 우선 순위" },
                        { "radioButton11", "언어의 자동 판단에 정규 표현을 우선한다 (추천)" },
                        { "checkBox6", "깨진 키릴 문자 디코더를 켭니다 (추천)" },

                        { "tabPage9", "길드 채팅" },
                        { "button8", "기본" },
                        { "button7", "재생" },
                        { "label7", "볼륨" },
                        { "checkBox2", "길드 채팅이 오면 소리를 울립니다 (추천)" },
                        { "Text", "설정" }
                    }
                }
            };
            return map[language_GP];
        }

        private static Dictionary<string, string> ChatDetail_Labels(string language_GP)
        {
            Dictionary<string, Dictionary<string, string>> map = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    Language_GP.EN,
                    new Dictionary<string, string>()
                    {
                        { "Text", "Chat Details" },
                        { "lblRow", "Row" },
                        { "cmdALL", "ALL" },
                        { "cmdID", "ID" },
                        { "cmdName", "Name" },
                        { "cmdText", "Chat" },
                        { "radioButton1", "Display the ID of the selected row" },
                        { "radioButton2", "Search and display the entered ID" },
                        { "btnIDSerch", "ID search" },
                        { "btnLogAnalysis", "Analyze chat logs" },
                        { "btnSTOP", "Stop analysis" },
                        { "lblNickName", "NickName" },
                        { "lblCharacterList", "Character list" },
                        { "lblRemarks", "Remarks" }
                    }
                }
                ,
                {
                    Language_GP.DE,
                    new Dictionary<string, string>()
                    {
                        { "Text", "Chat-Details" },
                        { "lblRow", "Row" },
                        { "cmdALL", "ALLE" },
                        { "cmdID", "ID" },
                        { "cmdName", "Name" },
                        { "cmdText", "Text" },
                        { "radioButton1", "Anzeige der ID der ausgewählten Zeile" },
                        { "radioButton2", "Suche und Anzeige der eingegebenen ID" },
                        { "btnIDSerch", "ID-Suche" },
                        { "btnLogAnalysis", "Chatprotokolle analysieren" },
                        { "btnSTOP", "Analyse beenden" },
                        { "lblNickName", "Spitzname" },
                        { "lblCharacterList", "Charakterliste" },
                        { "lblRemarks", "Bemerkungen" }
                    }
                }
                ,
                {
                    Language_GP.ES,
                    new Dictionary<string, string>()
                    {
                        { "Text", "Detalles del chat" },
                        { "lblRow", "Row" },
                        { "cmdALL", "ALL" },
                        { "cmdID", "ID" },
                        { "cmdName", "Name" },
                        { "cmdText", "Text" },
                        { "radioButton1", "Mostrar el ID de la fila seleccionada" },
                        { "radioButton2", "Buscar y mostrar el ID ingresado" },
                        { "btnIDSerch", "Búsqueda de ID" },
                        { "btnLogAnalysis", "Analizar registros de chat" },
                        { "btnSTOP", "Detener análisis" },
                        { "lblNickName", "apodo" },
                        { "lblCharacterList", "Lista de personajes" },
                        { "lblRemarks", "comentarios" }
                    }
                }
                ,
                {
                    Language_GP.FR,
                    new Dictionary<string, string>()
                    {
                        { "Text", "Détails de la discussion" },
                        { "lblRow", "Row" },
                        { "cmdALL", "ALL" },
                        { "cmdID", "ID" },
                        { "cmdName", "Name" },
                        { "cmdText", "Text" },
                        { "radioButton1", "Afficher l'ID de la ligne sélectionnée" },
                        { "radioButton2", "Rechercher et afficher l'ID saisi" },
                        { "btnIDSerch", "Recherche d'identité" },
                        { "btnLogAnalysis", "Analyser les journaux de discussion" },
                        { "btnSTOP", "Arrêter l'analyse" },
                        { "lblNickName", "surnom" },
                        { "lblCharacterList", "Liste des personnages" },
                        { "lblRemarks", "remarques" }
                    }
                }
                ,
                {
                    Language_GP.RU,
                    new Dictionary<string, string>()
                    {
                        { "Text", "チャット詳細" },
                        { "lblRow", "Row" },
                        { "cmdALL", "ALL" },
                        { "cmdID", "ID" },
                        { "cmdName", "Name" },
                        { "cmdText", "Text" },
                        { "radioButton1", "Показать идентификатор выбранной строки" },
                        { "radioButton2", "Найдите и отобразите введенный идентификатор" },
                        { "btnIDSerch", "Поиск ID" },
                        { "btnLogAnalysis", "Анализируйте журналы чата" },
                        { "btnSTOP", "Остановить анализ" },
                        { "lblNickName", "NickName" },
                        { "lblCharacterList", "Список персонажей" },
                        { "lblRemarks", "примечания" }
                    }
                }
                ,
                {
                    Language_GP.ZH_CN,
                    new Dictionary<string, string>()
                    {
                        { "Text", "聊天详情" },
                        { "lblRow", "线" },
                        { "cmdALL", "全文" },
                        { "cmdID", "ID" },
                        { "cmdName", "姓名" },
                        { "cmdText", "对话" },
                        { "radioButton1", "显示选中行的ID" },
                        { "radioButton2", "搜索并显示输入的ID" },
                        { "btnIDSerch", "身份证查询" },
                        { "btnLogAnalysis", "分析聊天记录" },
                        { "btnSTOP", "停止分析" },
                        { "lblNickName", "昵称" },
                        { "lblCharacterList", "角色列表" },
                        { "lblRemarks", "评论" }
                    }
                }
                ,
                {
                    Language_GP.ZH_TW,
                    new Dictionary<string, string>()
                    {
                        { "Text", "聊天詳情" },
                        { "lblRow", "線" },
                        { "cmdALL", "全文" },
                        { "cmdID", "ID" },
                        { "cmdName", "姓名" },
                        { "cmdText", "對話" },
                        { "radioButton1", "顯示選中行的ID" },
                        { "radioButton2", "搜索並顯示輸入的ID" },
                        { "btnIDSerch", "身份證查詢" },
                        { "btnLogAnalysis", "分析聊天記錄" },
                        { "btnSTOP", "停止分析" },
                        { "lblNickName", "暱稱" },
                        { "lblCharacterList", "角色列表" },
                        { "lblRemarks", "評論" }
                    }
                }
                ,
                {
                    Language_GP.JA,
                    new Dictionary<string, string>()
                    {
                        { "Text", "チャット詳細" },
                        { "lblRow", "行" },
                        { "cmdALL", "全文" },
                        { "cmdID", "ID" },
                        { "cmdName", "名前" },
                        { "cmdText", "会話" },
                        { "radioButton1", "選択された行のIDを表示する" },
                        { "radioButton2", "入力したIDを検索表示する" },
                        { "btnIDSerch", "ID検索" },
                        { "btnLogAnalysis", "チャットログを分析" },
                        { "btnSTOP", "分析を中止" },
                        { "lblNickName", "ニックネーム" },
                        { "lblCharacterList", "キャラクターリスト" },
                        { "lblRemarks", "備考" }
                    }
                }
                ,
                {
                    Language_GP.KO,
                    new Dictionary<string, string>()
                    {
                        { "Text", "채팅 세부정보" },
                        { "lblRow", "Row" },
                        { "cmdALL", "ALL" },
                        { "cmdID", "ID" },
                        { "cmdName", "Name" },
                        { "cmdText", "Text" },
                        { "radioButton1", "선택한 행의 ID 표시" },
                        { "radioButton2", "입력한 ID 검색 표시" },
                        { "btnIDSerch", "ID 검색" },
                        { "btnLogAnalysis", "채팅 로그 분석" },
                        { "btnSTOP", "분석 중단" },
                        { "lblNickName", "닉네임" },
                        { "lblCharacterList", "캐릭터 목록" },
                        { "lblRemarks", "비고" }
                    }
                }
            };
            return map[language_GP];
        }

        /*
         * ラベルを定義するよ。
         * 新しいラベルができたらここで定義してね。
         */


        public static string labelLanguage_GP()
        {
            switch (Language_GP.myLanguage_GP())
            {
                case "EN":
                case "DE":
                case "ES":
                case "FR":
                case "RU":
                case "ZH-CN":
                case "ZH-TW":
                case "JA":
                case "KO":
                default:
                    return Language_GP.myLanguage_GP();
            }
        }

        /*
         * ラベルを初期化するよ
         * 新しいラベルができたらここで定義してね
         */
        public static void initializeForm1(Form form)
        {
            Dictionary<string, string> labels = form1_Labels(labelLanguage_GP());
            initializeForm(form, labels);
        }

        /*
         * ラベルを初期化するよ
         * 新しいラベルができたらここで定義してね
         */
        public static void initializeForm2(Form form)
        {
            Dictionary<string, string> labels = form2_Labels(labelLanguage_GP());
            initializeForm(form, labels);
        }

        /*
         * ラベルを初期化するよ
         * 新しいラベルができたらここで定義してね
         */
        public static void initializeChatDetail(Form form)
        {
            Dictionary<string, string> labels = ChatDetail_Labels(labelLanguage_GP());
            initializeForm(form, labels);
        }
        
        private static void initializeForm(Form form, Dictionary<string, string> labels)
        {
            foreach (var label in labels)
            {
                if (label.Key == "Text")
                {
                    form.Text = labels["Text"];
                }
                else
                {
                    form.Controls.Find(label.Key, true)[0].Text = label.Value;
                }
            }
        }

    }

}
