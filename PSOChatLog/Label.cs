using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace PSOChatLog
{
    public class Label
    {

        private static string TITLE { get; } = "PSO Chat Log Ver " + Config.VERSION;

        private static Dictionary<string, Dictionary<string, string>> FORM1_LABELS = new Dictionary<string, Dictionary<string, string>>()
        {
                {
                    Language.EN,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "EtoJ" },
                        { "Button2", "JtoE" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language.DE,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "DtoE" },
                        { "Button2", "EtoD" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language.ES,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "StoE" },
                        { "Button2", "EtoS" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language.FR,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "FtoE" },
                        { "Button2", "EtoF" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language.RU,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "RtoE" },
                        { "Button2", "EtoR" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language.ZH_CN,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "CtoE" },
                        { "Button2", "EtoC" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language.ZH_TW,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "CtoE" },
                        { "Button2", "EtoC" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
                ,
                {
                    Language.JA,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "和英" },
                        { "Button2", "英和" },
                        { "Button3", "送信" },
                        { "Button4", "設定" }
                    }
                }
                ,
                {
                    Language.KO,
                    new Dictionary<string, string>()
                    {
                        { "Text", TITLE },
                        { "Button1", "KtoE" },
                        { "Button2", "EtoK" },
                        { "Button3", "Send" },
                        { "Button4", "SET" }
                    }
                }
            };

        private static Dictionary<string, Dictionary<string, string>> FORM2_LABELS = new Dictionary<string, Dictionary<string, string>>()
        {
            {
                    Language.EN,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "Setting" },
                        { "tabPage1", "Install" },
                        { "label6", "Please specify the full path to online.exe. " },
                        { "label8", "You can also set up an auto-get button or drag-and-drop."},
                        { "label9", "" },
                        { "Button2", "Auto-Get" },
                        { "label4", "Where to install Ephinea PSOBB" },
                        { "tabPage2", "Chat" },
                        { "checkBox1", "Press the spacebar and then chat." },
                        { "radioButton6", "Send text using only AHK" },
                        { "label11", "I can only send English alphabets, but the problem is less likely to occur" },
                        { "label12", "If you are familiar with AHK script maintenance, you can solve the problem relatively easily." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Default" },
                        { "radioButton7", "AHK is used to control IME, and the character string is sent from ChatLogTool itself." },
                        { "label19", "You can send sentences in a mixture of your native language and English" },
                        { "label15", "space-key Chat delay" },
                        { "label16", "Input mode change waiting time" },
                        { "label17", "Char-to-Char transmission wait" },
                        { "button10", "Default" },
                        { "radioButton8", "Use only the program of ChatLogTool itself to send the character string" },
                        { "label20", "I can\'t send sentences with mixed character codes, but it\'s the fastest" },
                        { "label21", "It is necessary to set the input mode on the PSOBB side each time" },
                        { "label18", "Char-to-Char transmission wait" },
                        { "button11", "Default" },
                        { "label22", "The unit of waiting time is (ms)" },
                        { "tabPage3", "View" },
                        { "Button3", "Default" },
                        { "label5", "Font" },
                        { "tabPage4", "Translate" },
                        { "button6", "Copy the Google Apps Scripts(GAS) automatic translation script to the clipboard." },
                        { "button5", "Get the URL of Google Apps Scripts(GAS) (browser opens)" },
                        { "button4", "Get DeepL API key (browser opens)" },
                        { "radioButton5", "I usually use DeepL, and when DeepL returns the \"original text\", I use Google Apps Scripts." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Translate using Google Apps Scripts (GAS)" },
                        { "radioButton3", "Translate using DeepL\'s \"DeepL API Pro\"" },
                        { "radioButton2", "Translate using DeepL\'s \"DeepL API Free\"" },
                        { "radioButton1", "Do not translate" },
                        { "tabPage5", "GuildChat" },
                        { "button8", "Default" },
                        { "button7", "Play" },
                        { "label7", "Volume" },
                        { "checkBox2", "Make a sound when guild chat comes" },
                        { "Text", "configuration" }
                    }
                }
                ,
                {
                    Language.DE,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "Einstellung" },
                        { "tabPage1", "Installieren" },
                        { "label6", "Bitte geben Sie den vollständigen Pfad zu online.exe an" },
                        { "label8", "Sie können es auch mit der Schaltfläche für die automatische Erfassung oder per Drag & Drop einstellen" },
                        { "label9", "" },
                        { "Button2", "Automatische Erfassung" },
                        { "label4", "Wo installiere ich Ephinea PSOBB" },
                        { "tabPage2", "Plaudern" },
                        { "checkBox1", "Drücke die Leertaste und dann chatte" },

                        { "radioButton6", "Send text using only AHK" },
                        { "label11", "I can only send English alphabets, but the problem is less likely to occur" },
                        { "label12", "If you are familiar with AHK script maintenance, you can solve the problem relatively easily." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Default" },
                        { "radioButton7", "AHK is used to control IME, and the character string is sent from ChatLogTool itself." },
                        { "label19", "You can send sentences in a mixture of your native language and English" },
                        { "label15", "space-key Chat delay" },
                        { "label16", "Input mode change waiting time" },
                        { "label17", "Char-to-Char transmission wait" },
                        { "button10", "Default" },
                        { "radioButton8", "Use only the program of ChatLogTool itself to send the character string" },
                        { "label20", "I can\'t send sentences with mixed character codes, but it\'s the fastest" },
                        { "label21", "It is necessary to set the input mode on the PSOBB side each time" },
                        { "label18", "Char-to-Char transmission wait" },
                        { "button11", "Default" },
                        { "label22", "The unit of waiting time is (ms)" },
/*
                        { "radioButton6", "Senden Sie Text nur mit AHK" },
                        { "label11", "Ich kann nur englische Alphabete senden, aber das Problem tritt weniger wahrscheinlich auf" },
                        { "label12", "Wenn Sie sich mit der AHK-Skriptpflege auskennen, können Sie das Problem relativ einfach lösen" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Standard" },
                        { "radioButton7", "AHK wird verwendet, um IME zu steuern, und die Zeichenfolge wird von ChatLogTool selbst gesendet" },
                        { "label19", "Sie können Sätze in einer Mischung aus Ihrer Muttersprache und Englisch senden" },
                        { "label15", "Chatverzögerung mit Leertaste" },
                        { "label16", "Eingabemoduswechsel warten" },
                        { "label17", "Übertragung zwischen Zeichen warten" },
                        { "button10", "Standard" },
                        { "radioButton8", "Verwenden Sie nur das Programm von ChatLogTool selbst, um die Zeichenfolge zu senden" },
                        { "label20", "Sätze mit gemischten Zeichencodes können nicht gesendet werden" },
                        { "label21", "Der Eingabemodus muss jedes Mal auf der PSOBB-Seite eingestellt werden" },
                        { "label17", "Übertragung zwischen Zeichen warten" },
                        { "button11", "Standard" },
                        { "label22", "Die Einheit der Wartezeit ist (ms)" },
*/
                        { "tabPage3", "Anzeige" },
                        { "Button3", "Standard" },
                        { "label5", "Schriftart" },
                        { "tabPage4", "Übersetzung" },
                        { "button6", "Kopieren Sie das automatische GAS-Übersetzungsskript in die Zwischenablage" },
                        { "button5", "Rufen Sie die URL von GAS ab (Browser wird geöffnet)" },
                        { "button4", "Holen Sie sich den DeepL-API-Schlüssel (Browser wird geöffnet)" },
                        { "radioButton5", "Normalerweise verwende ich DeepL," },
                        { "label10", "Google Apps-Skripts verwenden, wenn DeepL \"original\" zurückgibt" },
                        { "label3", "URL" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Mit Google Apps Scripts (GAS) übersetzen" },
                        { "radioButton3", "Übersetzen Sie mit DeepLs \"DeepL API Pro\"" },
                        { "radioButton2", "Übersetzen Sie mit DeepLs \"DeepL API Free\"" },
                        { "radioButton1", "Übersetze nicht" },
                        { "tabPage5", "Gildenchat" },
                        { "button8", "Standard" },
                        { "button7", "Reproduktion" },
                        { "label7", "Volumen" },
                        { "checkBox2", "Mach einen Ton, wenn der Gildenchat kommt" },
                        { "Text", "Konfiguration" }
                    }
                }
                ,
                {
                    Language.ES,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "Configuración" },
                        { "tabPage1", "Instalar" },
                        { "label6", "Especifique con la ruta completa a online.exe" },
                        { "label8", "También puede configurarlo con el botón de adquisición automática o arrastrar y soltar" },
                        { "label9", "" },
                        { "Button2", "Adquisición automática" },
                        { "label4", "Dónde instalar Ephinea PSOBB" },
                        { "tabPage2", "chat" },
                        { "checkBox1", "Presiona la barra espaciadora y luego chatea" },

                        { "radioButton6", "Envía mensajes de texto usando solo AHK" },
                        { "label11", "Solo puedo enviar alfabetos en inglés, pero es menos probable que ocurra el problema" },
                        { "label12", "Si está familiarizado con el mantenimiento del script AHK, puede resolver el problema con relativa facilidad." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Defecto" },
                        { "radioButton7", "AHK se utiliza para controlar IME y la cadena de caracteres se envía desde ChatLogTool." },
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
                        { "label22", "La unidad de tiempo de espera es (ms)." },

                        { "tabPage3", "monitor" },
                        { "Button3", "Defecto" },
                        { "label5", "fuente" },
                        { "tabPage4", "traducción" },
                        { "button6", "Copiar el script de traducción automática de Google Apps Scripts(GAS) al portapapeles" },
                        { "button5", "Obtener la URL de Google Apps Scripts(GAS) (se abre el navegador)" },
                        { "button4", "Obtener la clave de API de DeepL (se abre el navegador)" },
                        { "radioButton5", "Normalmente uso DeepL, y cuando DeepL devuelve el \"texto original\", utilizo Google Apps Scripts." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Traducir usando Google Apps Scripts (GAS)" },
                        { "radioButton3", "Traducir usando \"DeepL API Pro\" de DeepL" },
                        { "radioButton2", "Traduce usando \"DeepL API Free\" de DeepL" },
                        { "radioButton1", "No traducir" },
                        { "tabPage5", "Chat del gremio" },
                        { "button8", "Defecto" },
                        { "button7", "reproducción" },
                        { "label7", "volumen" },
                        { "checkBox2", "Haz un sonido cuando llegue el chat del gremio" },
                        { "Text", "Configuración" }
                    }
                }
                ,
                {
                    Language.FR,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "Configuration" },
                        { "tabPage1", "installer" },
                        { "label6", "Veuillez spécifier avec le chemin complet vers online.exe" },
                        { "label8", "Vous pouvez également le paramétrer avec le bouton d'acquisition automatique ou par glisser-déposer" },
                        { "label9", "" },
                        { "Button2", "Acquisition automatique" },
                        { "label4", "Où installer Ephinea PSOBB" },
                        { "tabPage2", "discuter" },
                        { "checkBox1", "Appuyez sur la barre d'espace puis discutez" },

                        { "radioButton6", "Envoyer du texte en utilisant uniquement AHK" },
                        { "label11", "Je ne peux envoyer que des alphabets anglais, mais le problème est moins susceptible de se produire" },
                        { "label12", "Si vous êtes familiarisé avec la maintenance des scripts AHK, vous pouvez résoudre le problème assez facilement." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Défaut" },
                        { "radioButton7", "AHK est utilisé pour contrôler l'IME et la chaîne de caractères est envoyée depuis ChatLogTool lui-même." },
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
                        { "label22", "L'unité de temps d'attente est (ms)." },

                        { "tabPage3", "affichage" },
                        { "Button3", "Défaut" },
                        { "label5", "Font" },
                        { "tabPage4", "Traduction" },
                        { "button6", "Copier le script de traduction automatique Google Apps Scripts(GAS) dans le presse-papiers" },
                        { "button5", "Obtenez l'URL de Google Apps Scripts(GAS) (le navigateur s'ouvre)" },
                        { "button4", "Obtenir la clé API DeepL (le navigateur s'ouvre)" },
                        { "radioButton5", "J'utilise généralement DeepL, et lorsque DeepL renvoie le \"texte d\'origine\", j\'utilise les scripts Google Apps." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Traduire à l'aide de Google Apps Scripts (GAS)" },
                        { "radioButton3", "Traduire à l'aide de \"DeepL API Pro\" de DeepL" },
                        { "radioButton2", "Traduire en utilisant \"DeepL API Free\" de DeepL" },
                        { "radioButton1", "Ne traduisez pas" },
                        { "tabPage5", "Chat de guilde" },
                        { "button8", "Défaut" },
                        { "button7", "la reproduction" },
                        { "label7", "le volume" },
                        { "checkBox2", "Faire un son lorsque le chat de guilde arrive" },
                        { "Text", "Configuration" }
                    }
                }
                ,
                {
                    Language.RU,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "Конфигурация" },
                        { "tabPage1", "установить" },
                        { "label6", "Укажите полный путь к online.exe" },
                        { "label8", "Кнопка автоматического захвата или" },
                        { "label9", "Вы также можете установить перетаскиванием" },
                        { "Button2", "Автоматическое приобретение" },
                        { "label4", "Где установить Ephinea PSOBB" },
                        { "tabPage2", "чат" },
                        { "checkBox1", "Нажмите пробел, а затем поговорите" },

                        { "radioButton6", "Отправить текст, используя только AHK" },
                        { "label11", "Могут быть отправлены только английские алфавиты" },
                        { "label12", "Если вы посмотрите на сценарий AHK, вы можете решить проблему относительно легко." },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "Дефолт" },
                        { "radioButton7", "AHK используется для управления IME, а строка символов отправляется из самого ChatLogTool." },
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
                        { "label22", "Единица времени ожидания - (мс)." },

                        { "tabPage3", "отображать" },
                        { "Button3", "Дефолт" },
                        { "label5", "Шрифт" },
                        { "tabPage4", "перевод" },
                        { "button6", "Скопировать скрипт автоматического перевода GAS в буфер обмена" },
                        { "button5", "Получить URL-адрес Google Apps Scripts(GAS) (открывается браузер)" },
                        { "button4", "Получить ключ DeepL API (открывается браузер)" },
                        { "radioButton5", "Я обычно использую DeepL," },
                        { "label10", "Используйте скрипты Google Apps, когда DeepL возвращает \"оригинал\"" },
                        { "label3", "URL" },
                        { "label2", "КЛЮЧ API" },
                        { "label1", "КЛЮЧ API" },
                        { "radioButton4", "Перевести с помощью скриптов Google Apps (GAS)" },
                        { "radioButton3", "Переводите с помощью DeepL API Pro" },
                        { "radioButton2", "Переводите с помощью DeepL API Free DeepL" },
                        { "radioButton1", "Не переводить" },
                        { "tabPage5", "Чат гильдии" },
                        { "button8", "Дефолт" },
                        { "button7", "воспроизведение" },
                        { "label7", "объем" },
                        { "checkBox2", "Сделайте звук, когда появится чат гильдии" },
                        { "Text", "Конфигурация" }
                    }
                }
                ,
                {
                    Language.ZH_CN,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "配置" },
                        { "tabPage1", "安装" },
                        { "label6", "请使用 online.exe 的完整路径指定" },
                        { "label8", "也可以通过自动获取按钮或拖放设置" },
                        { "label9", "" },
                        { "Button2", "自动采集" },
                        { "label4", "在哪里安装 Ephinea PSOBB" },
                        { "tabPage2", "聊天" },
                        { "checkBox1", "按空格键然后聊天" },

                        { "radioButton6", "仅使用 AHK 发送文本" },
                        { "label11", "我只能发送英文字母，但出现问题的可能性较小" },
                        { "label12", "如果您熟悉AHK脚本维护，则可以相对轻松地解决问题" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "默认" },
                        { "radioButton7", "AHK用于控制输入法，字符串由ChatLogTool自身发送" },
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
                        { "label22", "等待时间的单位是（ms）" },

                        { "tabPage3", "展示" },
                        { "Button3", "默认" },
                        { "label5", "字体" },
                        { "tabPage4", "翻译" },
                        { "button6", "将 Google Apps Scripts(GAS) 自动翻译脚本复制到剪贴板" },
                        { "button5", "获取 Google Apps Scripts(GAS) 的 URL（浏览器打开）" },
                        { "button4", "获取 DeepL API 密钥（浏览器打开）" },
                        { "radioButton5", "我通常使用 DeepL，当 DeepL 返回“原始文本”时，我使用 Google Apps Scripts" },
                        { "label10", "" },
                        { "label3", "网址" },
                        { "label2", "接口密钥" },
                        { "label1", "接口密钥" },
                        { "radioButton4", "使用 Google Apps 脚本 (GAS) 进行翻译" },
                        { "radioButton3", "使用 DeepL 的“DeepL API Pro”进行翻译" },
                        { "radioButton2", "使用 DeepL 的“DeepL API Free”进行翻译" },
                        { "radioButton1", "不翻译" },
                        { "tabPage5", "公会聊天" },
                        { "button8", "默认" },
                        { "button7", "再生产" },
                        { "label7", "体积" },
                        { "checkBox2", "公会聊天来时发出声音" },
                        { "Text", "配置" }
                    }
                }
                ,
                {
                    Language.ZH_TW,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "配置" },
                        { "tabPage1", "安裝" },
                        { "label6", "請使用 online.exe 的完整路徑指定" },
                        { "label8", "也可以通過自動採集按鈕或拖拽設置" },
                        { "label9", "" },
                        { "Button2", "自動採集" },
                        { "label4", "在哪里安裝 Ephinea PSOBB" },
                        { "tabPage2", "聊天" },
                        { "checkBox1", "按空格鍵然後聊天" },

                        { "radioButton6", "僅使用 AHK 發送文本" },
                        { "label11", "我只能發送英文字母，但出現問題的可能性較小" },
                        { "label12", "如果您熟悉AHK腳本維護，則可以相對輕鬆地解決問題" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "默認" },
                        { "radioButton7", "AHK用於控制輸入法，字符串由ChatLogTool自身發送" },
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
                        { "label22", "等待時間的單位是（ms）" },

                        { "tabPage3", "展示" },
                        { "Button3", "默認" },
                        { "label5", "字體" },
                        { "tabPage4", "翻譯" },
                        { "button6", "將 Google Apps Scripts(GAS) 自動翻譯腳本複製到剪貼板" },
                        { "button5", "獲取 Google Apps Scripts(GAS) 的 URL（瀏覽器打開）" },
                        { "button4", "獲取 DeepL API 密鑰（瀏覽器打開）" },
                        { "radioButton5", "我通常使用 DeepL，當 DeepL 返回“原始文本”時，我使用 Google Apps Scripts" },
                        { "label10", "" },
                        { "label3", "網址" },
                        { "label2", "接口密鑰" },
                        { "label1", "接口密鑰" },
                        { "radioButton4", "使用 Google Apps 腳本 (GAS) 進行翻譯" },
                        { "radioButton3", "使用 DeepL 的“DeepL API Pro”進行翻譯" },
                        { "radioButton2", "使用 DeepL 的“DeepL API Free”進行翻譯" },
                        { "radioButton1", "不翻譯" },
                        { "tabPage5", "公會聊天" },
                        { "button8", "默認" },
                        { "button7", "再生產" },
                        { "label7", "體積" },
                        { "checkBox2", "公會聊天來時發出聲音" },
                        { "Text", "配置" }
                    }
                }
                ,
                {
                    Language.JA,
                    new Dictionary<string, string>()
                    {
                        { "Text", "設定" },
                        { "tabPage1", "インストール" },
                        { "label4", "Ephinea PSOBBのインストール先" },
                        { "Button2", "自動取得" },
                        { "label6", "online.exeまでのフルパスで指定してください" },
                        { "label8", "自動取得ボタンか、ドラッグアンドドロップでも設定できます" },
                        { "label9", "" },
                        { "tabPage2", "チャット" },
                        { "checkBox1", "スペースキーを押してからチャットをする" },

                        { "radioButton6", "AHKのみを利用して、テキスト送信する" },
                        { "label11", "英語のアルファベットしか送信できませんが、問題は発生しづらいです" },
                        { "label12", "詳しい方がAHKスクリプトを保守すれば、割と簡単に問題解決も図れます" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "デフォルト" },
                        { "radioButton7", "IMEのコントロールにAHKを利用して、文字列はChatLogTool本体から送信する" },
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
                        { "label22", "待ち時間(Wait)は、全て単位が(ms)です" },

                        { "tabPage3", "表示" },
                        { "label5", "Font" },
                        { "Button3", "デフォルト" },
                        { "tabPage4", "翻訳" },
                        { "radioButton1", "翻訳をしない" },
                        { "radioButton2", "DeepLの「DeepL API Free」を利用して翻訳する" },
                        { "label1", "API KEY" },
                        { "button4", "DeepLのAPIキーを取得する(ブラウザが開きます)" },
                        { "radioButton3", "DeepLの「DeepL API Pro」を利用して翻訳する" },
                        { "label2", "API KEY" },
                        { "radioButton4", "Google Apps Scripts(GAS)を利用して翻訳する" },
                        { "label3", "URL" },
                        { "button6", "GASの自動翻訳スクリプトをクリップボードにコピーする" },
                        { "button5", "GASのURLを取得する(ブラウザが開きます)" },
                        { "radioButton5", "普段はDeepLを利用し、DeepLが「原文」を返した時にGoogle Apps Scriptsを利用する" },
                        { "label10", "" },
                        { "tabPage5", "ギルドチャット" },
                        { "button8", "デフォルト" },
                        { "button7", "再生" },
                        { "label7", "ボリューム" },
                        { "checkBox2", "ギルドチャットが来たら音を鳴らす" },
                        { "Button1", "設定" }
                    }
                }
                ,
                {
                    Language.KO,
                    new Dictionary<string, string>()
                    {
                        { "Button1", "설정" },
                        { "tabPage1", "설치" },
                        { "label6", "online.exe까지의 전체 경로로 지정하십시오" },
                        { "label8", "자동 획득 버튼이나 드래그 앤 드롭으로 설정할 수 있습니다" },
                        { "label9", "" },
                        { "Button2", "자동 취득" },
                        { "label4", "Ephinea PSOBB가 설치된 위치" },
                        { "tabPage2", "채팅" },
                        { "checkBox1", "스페이스바를 누른 후 채팅하기" },

                        { "radioButton6", "AHK만 사용하여 텍스트 전송" },
                        { "label11", "영어 알파벳만 보낼 수 있지만 문제가 발생하기 어렵습니다" },
                        { "label12", "자세한 쪽이 AHK 스크립트를 보수하면, 비교적 간단하게 문제 해결도 도모할 수 있습니다" },
                        { "label13", "AHK SetKeyDelay [Delay]" },
                        { "label14", "AHK SetKeyDelay [PressDuration]" },
                        { "button9", "기본" },
                        { "radioButton7", "IME 컨트롤에 AHK를 사용하여 문자열은 ChatLogTool 본문에서 전송" },
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
                        { "label22", "대기 시간은 모두 단위가 (ms)입니다" },

                        { "tabPage3", "표시" },
                        { "Button3", "기본" },
                        { "label5", "Font" },
                        { "tabPage4", "번역" },
                        { "button6", "Google Apps Scripts(GAS) 자동 번역 스크립트를 클립보드에 복사" },
                        { "button5", "Google Apps Scripts(GAS) URL 가져 오기 (브라우저가 열립니다)" },
                        { "button4", "DeepL의 API 키 가져 오기 (브라우저가 열립니다) 2021/11/10 현재 한국어를 지원하지 않음" },
                        { "radioButton5", "평소에는 DeepL을 이용하고 DeepL이 '원문'을 반환했을 때 Google Apps Scripts를 이용한다." },
                        { "label10", "" },
                        { "label3", "URL" },
                        { "label2", "API KEY" },
                        { "label1", "API KEY" },
                        { "radioButton4", "Google Apps Scripts (GAS)를 사용하여 번역" },
                        { "radioButton3", "DeepL의 \"DeepL API Pro\"를 이용하여 번역하기" },
                        { "radioButton2", "DeepL의 \"DeepL API Free\"를 이용하여 번역하기" },
                        { "radioButton1", "번역 안함" },
                        { "tabPage5", "길드 채팅" },
                        { "button8", "기본" },
                        { "button7", "재생" },
                        { "label7", "볼륨" },
                        { "checkBox2", "길드 채팅이 오면 소리를 울립니다" },
                        { "Text", "설정" }
                    }
                }
            };

        private static Dictionary<string, Dictionary<string, string>> CHATDETAILS_LABELS = new Dictionary<string, Dictionary<string, string>>()
        {
                {
                    Language.EN,
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
                    Language.DE,
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
                    Language.ES,
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
                    Language.FR,
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
                    Language.RU,
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
                        { "lblNickName", "прозвище" },
                        { "lblCharacterList", "Список персонажей" },
                        { "lblRemarks", "примечания" }
                    }
                }
                ,
                {
                    Language.ZH_CN,
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
                    Language.ZH_TW,
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
                    Language.JA,
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
                    Language.KO,
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

        /*
         * ラベルを定義するよ。
         * 新しいラベルができたらここで定義してね。
         */
        public static string labelLanguage()
        {
            switch (Language.myLanguage())
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
                    return Language.myLanguage();
            }
        }

        /*
         * ラベルを初期化するよ
         */
        public static void initializeForm1(Form form)
        {

            // ディクショナリに規定の言語がない場合、デフォルトのラベルを使う
            if (FORM1_LABELS.ContainsKey(labelLanguage()))
            {
                initializeForm(form, FORM1_LABELS[labelLanguage()]);
            } else
            {
                initializeForm(form, FORM1_LABELS[Language.EN]);
            }
        }

        /*
         * ラベルを初期化するよ
         */
        public static void initializeForm2(Form form)
        {
            // ディクショナリに規定の言語がない場合、デフォルトのラベルを使う
            if (FORM1_LABELS.ContainsKey(labelLanguage()))
            {
                initializeForm(form, FORM2_LABELS[labelLanguage()]);
            }
            else
            {
                initializeForm(form, FORM2_LABELS[Language.EN]);
            }
        }

        /*
         * ラベルを初期化するよ
         */
        public static void initializeChatDetail(Form form)
        {
            // ディクショナリに規定の言語がない場合、デフォルトのラベルを使う
            if (FORM1_LABELS.ContainsKey(labelLanguage()))
            {
                initializeForm(form, CHATDETAILS_LABELS[labelLanguage()]);
            }
            else
            {
                initializeForm(form, FORM2_LABELS[Language.EN]);
            }
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
