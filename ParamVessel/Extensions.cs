using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MeowsBetterParamEditor
{
    public static class Extensions
    {
        public static TAncestor FindAncestor<TAncestor>(this UIElement ui)
            where TAncestor : DependencyObject
        {
            DependencyObject result = (DependencyObject)ui;
            do
            {
                result = VisualTreeHelper.GetParent(ui);
            }
            while (result.GetType() != typeof(TAncestor));
            return (TAncestor)result;
        }
    }
}
