using Variables.RealmsInfo;

namespace BLD
{
    public class DataStore_Realm
    {
        public readonly CurrentRealmVariable playerRealm = new CurrentRealmVariable();
        public readonly BaseCollection<RealmModel> realmsInfo = new BaseCollection<RealmModel>();
    }
}