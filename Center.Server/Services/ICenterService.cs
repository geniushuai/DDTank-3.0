using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Center.Server
{
    // NOTE: If you change the interface name "ICenterService" here, you must also update the reference to "ICenterService" in App.config.
    [ServiceContract]
    public interface ICenterService
    {
        [OperationContract]
        List<ServerData> GetServerList();

        [OperationContract]
        bool ChargeMoney(int userID, string chargeID);

        [OperationContract]
        bool SystemNotice(string msg);

        [OperationContract]
        bool KitoffUser(int playerID, string msg);

        [OperationContract]
        bool ReLoadServerList();

        [OperationContract]
        bool MailNotice(int playerID);

        [OperationContract]
        bool ActivePlayer(bool isActive);

        [OperationContract]
        bool CreatePlayer(int id, string name, string password, bool isFirst);

        [OperationContract]
        bool ValidateLoginAndGetID(string name, string password, ref int userID, ref bool isFirst);

        [OperationContract]
        bool AASUpdateState(bool state);

        [OperationContract]
        int AASGetState();

        [OperationContract]
        int ExperienceRateUpdate(int serverId);

        [OperationContract]
        int NoticeServerUpdate(int serverId,int type);

        [OperationContract]
        bool UpdateConfigState(int type ,bool state);

        [OperationContract]
        int GetConfigState(int type);

        [OperationContract]
        bool Reload(string type);
    }
}
