using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bussiness.Protocol
{
    public enum eReloadType
    {
        ball = 0x01,
        map = 0x02,
        fallitem = 0x03,
        questitem = 0x04,
        mapserver = 0x05,
        fallprop = 0x06,
        prop = 0x07,
        item = 0x08,
        quest = 0x09,
        fusion = 0x0a,
        server = 0x0b,
        rate = 0x0c,
        consortia = 0x0d,
        shop = 0x0e,
        GP = 0x0f,
        fight = 0x10,
        dailyaward = 0x11,
        language = 0x12,
    }
}
