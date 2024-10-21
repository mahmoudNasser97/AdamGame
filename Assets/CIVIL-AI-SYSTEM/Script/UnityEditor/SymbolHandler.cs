#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace AISystem.EditorSystems
{
    public class SymbolHandler
    {
        string currentSymbols;
        NamedBuildTarget namedBuildTarget;

        public SymbolHandler()
        {
            GetNamedBuildTarget();
            LoadCurrentSymbols();
        }

        public NamedBuildTarget GetNamedBuildTarget()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);
            namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

            return namedBuildTarget;
        }

        public string LoadCurrentSymbols()
        {
            string symbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            currentSymbols = symbols;
            return symbols;
        }

        public bool UpdateSymbols(string symbols = null)
        {
            if (symbols == null)
            {
                symbols = currentSymbols;
            }

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols);

            return true;
        }

        public bool RemoveSymbol(string toRemove)
        {
            currentSymbols = currentSymbols.Replace(toRemove + ";", "");
            currentSymbols = currentSymbols.Replace(toRemove, "");

            return true;
        }

        public bool RemoveSymbol(string[] toRemove)
        {
            foreach (var entry in toRemove)
            {
                currentSymbols = currentSymbols.Replace(entry + ";", "");
                currentSymbols = currentSymbols.Replace(entry, "");
            }

            return true;
        }

        public bool AddSymbol(string symbol)
        {
            currentSymbols += ";" + symbol;

            return true;
        }

        public bool CheckForDomain(string name)
        {
            var result = AppDomain.CurrentDomain
                .GetAssemblies();

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(t => t.FullName.Split(",")[0] == name) != null;
        }
    }
}
#endif
