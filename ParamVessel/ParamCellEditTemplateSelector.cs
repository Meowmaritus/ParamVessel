using MeowDSIO.DataTypes.PARAM;
using MeowDSIO.DataTypes.PARAMDEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MeowsBetterParamEditor
{
    public class ParamCellEditTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                if (item is ParamCellValueRef cell)
                {
                    //bool
                    if (cell.Def.ValueBitCount == 1)
                        return null;

                    switch (cell.Def.InternalValueType)
                    {
                        default:
                            return element.FindResource("CellTemplateTextBox_Edit") as DataTemplate;
                        case ParamTypeDef.ATK_PARAM_BOOL:
                        case ParamTypeDef.EQUIP_BOOL:
                        case ParamTypeDef.ITEMLOT_CUMULATE_RESET:
                        case ParamTypeDef.ITEMLOT_ENABLE_LUCK:
                        case ParamTypeDef.MAGIC_BOOL:
                        case ParamTypeDef.NPC_BOOL:
                        case ParamTypeDef.ON_OFF:
                        case ParamTypeDef.SP_EFFECT_BOOL:
                            return null;
                    }
                }
            }

            return null;
        }
    }
}
