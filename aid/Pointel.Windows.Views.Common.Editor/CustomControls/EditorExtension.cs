using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Pointel.Windows.Views.Common.Editor.CustomControls
{
	internal static class EditorExtension
	{
		public static IEnumerable<T> FindChildren<T>(this DependencyObject source) where T : DependencyObject
		{
			if (source != null)
			{
				IEnumerable<DependencyObject> childObjects = source.GetChildObjects();
				foreach (DependencyObject current in childObjects)
				{
					if (current != null && current is T)
					{
						yield return (T)((object)current);
					}
					foreach (T current2 in current.FindChildren<T>())
					{
						yield return current2;
					}
				}
			}
			yield break;
		}
		public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent)
		{
			if (parent != null)
			{
				if (parent is ContentElement || parent is FrameworkElement)
				{
					foreach (object current in LogicalTreeHelper.GetChildren(parent))
					{
						DependencyObject dependencyObject = current as DependencyObject;
						if (dependencyObject != null)
						{
							yield return (DependencyObject)current;
						}
					}
				}
				else
				{
					int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
					for (int i = 0; i < childrenCount; i++)
					{
						yield return VisualTreeHelper.GetChild(parent, i);
					}
				}
			}
			yield break;
		}
	
	}
}
