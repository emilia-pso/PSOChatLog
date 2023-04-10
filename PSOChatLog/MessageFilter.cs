using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSOChatLog
{
    internal class MessageFilter: IMessageFilter
    {
        enum WindowMessage
        {
            WM_TIMER = 0x0113,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            SB_BOTTOM = 0x07,
            SB_ENDSCROLL = 0x08,
            SB_LINEDOWN = 0x01,
            SB_LINEUP = 0x00,
            SB_PAGEDOWN = 0x03,
            SB_PAGEUP = 0x02,
            SB_THUMBPOSITION = 0x04,
            SB_THUMBTRACK = 0x005,
            SB_TOP =0x06
        }

        public bool PreFilterMessage(ref Message m)
        {
            //if (
            //    m.Msg == (int)WindowMessage.WM_KEYDOWN
            //    || m.Msg == (int)WindowMessage.WM_KEYUP
            //    || m.Msg == (int)WindowMessage.WM_CHAR
            //    )
            //{
            do
            {
                if (m.Msg == (int)WindowMessage.WM_TIMER)
                {
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_BOTTOM)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_ENDSCROLL)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_LINEDOWN)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_LINEUP)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_PAGEDOWN)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_PAGEUP)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_THUMBPOSITION)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_THUMBTRACK)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                if (m.Msg == (int)WindowMessage.SB_TOP)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
                    break;
                }
                //System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}", m.Msg));
            } while (false);
            //}
            return false;
        }
    }
}
