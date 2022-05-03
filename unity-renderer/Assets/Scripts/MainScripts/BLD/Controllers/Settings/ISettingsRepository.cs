using System;

namespace BLD.SettingsCommon
{
    public interface ISettingsRepository<T>
    {
        event Action<T> OnChanged;
        T Data { get; }
        void Apply(T settings);
        void Reset();
        void Save();
        bool HasAnyData();
    }
}