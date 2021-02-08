using System;
using System.Dynamic;

namespace Outils
{
    public class Ligne : DynamicObject
        {
            public object[] _Valeurs;
            public String[] _Proprietes;

            public object this[String champ]
            {
                get
                {
                    // Recherche l'index du champ
                    for (int i = 0; i < _Proprietes.Length; i++)
                    {
                        // Si le nom de la propriété est trouvée, donne la valeur
                        if (_Proprietes[i] == champ)
                            return _Valeurs[i];
                    }

                    // Si il ne le trouve pas
                    return null;
                    //throw new Exception("Ligne : Champ innexistant !");
                }

                set
                {
                    // Recherche l'index du champ
                    for (int i = 0; i < _Proprietes.Length; i++)
                    {
                        // Si le nom de la propriété est trouvée, attribue nouvelle la valeur
                        if (_Proprietes[i] == champ)
                            _Valeurs[i] = value;
                    }
                }
            }

            public object this[int index]
            {
                get
                {
                    return _Valeurs[index];
                }

                set
                {
                    _Valeurs[index] = value;
                }
            }


            /********************************************************************************************************************************************************************************************************************************************************************************
            * 
            * Demande d'une valeur de proprieté (pour binding par exemple)
            * 
            ***********************************************************************************************************************************************************************************************************************************************************************************/

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                // Recherche l'index du champ
                for (int i = 0; i < _Proprietes.Length; i++)
                {
                    // Si le nom de la propriété est trouvée
                    if (_Proprietes[i] == binder.Name)
                    {
                        // Donne la valeur
                        result = _Valeurs[i];
                        return true;
                    }
                }

                // La propriété n'a pas été trouvée
                result = null;
                return false;
            }


            /********************************************************************************************************************************************************************************************************************************************************************************
            * 
            * Attribution d'une valeur de proprieté (pour binding par exemple)
            * 
            ***********************************************************************************************************************************************************************************************************************************************************************************/
            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                // Recherche l'index du champ
                for (int i = 0; i < _Proprietes.Length; i++)
                {
                    // Si le nom de la propriété est trouvée
                    if (_Proprietes[i] == binder.Name)
                    {
                        // Donne la valeur
                        _Valeurs[i] = value;
                        return true;
                    }
                }

                // La propriété n'a pas été trouvée
                return true;
            }

        }
}
