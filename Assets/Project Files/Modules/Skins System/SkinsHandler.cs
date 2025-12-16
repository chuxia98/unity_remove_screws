using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Skins Handler", menuName = "Content/Skins/Skins Handler")]
    public class SkinsHandler : ScriptableObject
    {
        [SerializeField] AbstractSkinsProvider[] skinProviders;
        public AbstractSkinsProvider[] SkinsProviders => skinProviders;

        public int ProvidersCount => skinProviders.Length;

        public AbstractSkinsProvider GetSkinsProvider(int index)
        {
            return skinProviders[index];
        }

        public AbstractSkinsProvider GetSkinsProvider(System.Type providerType)
        {
            if (!skinProviders.IsNullOrEmpty())
            {
                foreach (AbstractSkinsProvider skinProvider in skinProviders)
                {
                    if (skinProvider.GetType() == providerType)
                        return skinProvider;
                }
            }

            return null;
        }

        public bool HasSkinsProvider(System.Type providerType)
        {
            if (!skinProviders.IsNullOrEmpty())
            {
                foreach (AbstractSkinsProvider skinProvider in skinProviders)
                {
                    if (skinProvider.GetType() == providerType)
                        return true;
                }
            }

            return false;
        }

        public bool HasSkinsProvider(AbstractSkinsProvider provider)
        {
            if(!skinProviders.IsNullOrEmpty())
            {
                foreach (AbstractSkinsProvider skinProvider in skinProviders)
                {
                    if (skinProvider == provider)
                        return true;
                }
            }

            return false;
        }
    }
}
