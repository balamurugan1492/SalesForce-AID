   M S B A M L   `   `   `  ���� I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=null?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers  c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 1    +StatTickerFive.Views.StatisticsConfigWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Ge9http://schemas.microsoft.com/expression/2010/interactions     5c-http://schemas.microsoft.com/wpf/2008/toolkit  Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4UxwtkJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  0local$clr-namespace:StatTickerFive.Helpers  XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  -   $��
UserConfig��  � Title$  UserLevelConfigurations��$
��Auto��$
��Auto�� � SizeToContent$ WidthAndHeight=� � Topmost .   � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png������ *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ 	Activated��.  (System.Windows.Interactivity.TriggerBase	   Actions.	 6  0System.Windows.Interactivity.InvokeCommandAction  
   Command
 �� *ActivatedCommand+  $ Deactivated��.	   
 �� *DeactivateCommand+  $ MouseLeftButtonDown��.	   
 �� *	DragCmd+   CommandParameter ��  �� ElementName$ 
UserConfig�� �� Path$ Title�  $ Loaded��.	   
 �� *WinLoadCommand+ � 	Resources  �� MergedDictionaries �� Source    booleanToVisibilityConverter,   &StatTickerFive.Helpers.LengthConverter  dataGridLengthConverter��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%   &	        &	      ��   .���� $
��0.89q� �� CornerRadius$ 2��$
��Auto��$
��Auto��
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $
��Auto��$	��705�� � ColumnDefinitions �� $	��525 ��� ���� *	GridWidth+ � RowDefinitions �� $��27 ��� $��* �.+��� $��0��$��27��$��0��$
��Auto������ *TitleBackground+���� *	TitleSpan+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�._��  �� $
��Auto ��� $��* ��� $��25 �.+��� $��0��$��8,3,0,0q�$��Statistics Configuration��$
��Left=�$��Stretch=�$
��Bold"�$��14(����� *TitleForeground+�� $��1��$��0,3,5,0q�$��Right=�$
��Bold"�$��14(����� *ApplicationName+���� *TitleForeground+�� -   $��btnExit��$��2��$��Center=�$��20��$��20��  AdminCloseButton#��[ $��2q����� *ConfigCancelCmd+�� $��1��
�������$��1q����#D6D7D6$	��669��$	��518��._��� -   $��StatisticsTabControl��$��1��$��2��$	��664��$	��515�� �� TabStripPlacement$	 Top=�.@��� -   $��StatisticsTabItem��$��Level Configurations��$
��Left=�$
��Auto��.���� 
�������$��1q�$��2q����#D6D7D6$	��625��$	��510��._�� $
��Auto��$
��Auto�� �� $��40 ��� $��* � �� $��29 ��� $��* �.+��� ���#D6D7D6
�������$��1q�$��0��$��0��$��3��$��36��$	��503��$��2q�._�� $	��495��$��5q�.+��  �� $��75 ��� $��* �.+��� $��Search Object��$��0��$
��auto��$��Center=�$
��Left=�$��0q�1  +Microsoft.Windows.Controls.WatermarkTextBox  $	��300��$��1��$
��Left=�   	Watermark$ Agent (or) AgentGroup������ *SearchValue+$ PropertyChanged=�$ TwoWay=�.��   $ TextChanged��.	   
 �� *SearchObjectCommand+.���� ���#D6D7D6
�������$	��Top=�$��1q�$��1��$��0��$��3��$	��577��$	��503��$��2,2,2,0q�._��  �� $��* ��� $��30 �.+��� $	��0.5q�$��0��$��2q�$	��495������ *TitleBackground+y��� *GridVisible+._��  �� $��25 ��� $��* �.+��� $��0��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
��������� $��0��$��1��$��Available Objects��$��Center=�$
��Bold"�$��Center=�$��12(�$��Calibri)�
���   ��� $	��492��$��1��.��� $	��492��$
��Auto��.+�&   System.Windows.Controls.DataGrid  -   $��ObjectsGridView��   CanUserReorderColumns .  $��-2,0,0,0q�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6
�������   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$
 None=�   AutoGenerateColumns .      CanUserAddRows  .    
CustomGrid#��[ $	��495��$	��545��$!   HorizontalScrollBarVisibility$! Disabled=�""   VerticalScrollBarVisibility$" Disabled=�#   
IsReadOnly# .  ���� *ObjectCollection+$ �� NotifyOnTargetUpdated$ . $ PropertyChanged=�$ TwoWay=�%   Columns% 4  .System.Windows.Controls.DataGridTemplateColumn  ,	  &System.Windows.Controls.DataGridColumn& 	  Width5
  /System.Windows.Controls.DataGridLengthConverter$& 25
 '   CellTemplate' �� .R��� $��chkbox��$��5q�$��20��( d� 	IsChecked( �� *isGridChecked+$ TwoWay=�$ PropertyChanged=�   $ Checked��.	   
 �� *#!DataContext.ObjectSelectedCommand+) �� RelativeSource) �� * �� AncestorType$* DataGridL� ��   $ 	Unchecked��.	   
 �� *%#DataContext.ObjectDeselectedCommand+) �� $* DataGridL� ��   $	& 450
 + 	  Header$+ Available Objects��' �� .R��� $��
lblObjName��, �� Padding$, 5,0,0,0q����� *
ObjectName+$ TwoWay=�$ PropertyChanged=�- � ToolTip- �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��ObjectsAvail��$��0��$, 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
ObjectName+$ TwoWay=�$ PropertyChanged=�� $��1�� �� $	��235 ��� $��* �.+��� $��0��  NormalButton#��[ $
��Save��$��23��$��75��$��Right=�$��0,5,2,2q����� *SaveStatistics+- �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��SaveSettings��$��Save Settings��$��0��$, 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��#��[ $
��Skip��$��23��$��75��$
��Left=�$��2,5,0,2q����� *ConfigSkipCmd+- �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��SkipSettings��$��Skip Settings��$��0��$, 	10,5,10,0q�$
��Bold"�$��Calibri)��� ���#D6D7D6$	��Top=�
�������$��1q�$��1��$��1��$	��675��$	��495��$��2,2,2,0q�._��� $	��0.5q�$��2��$��0��$��2��$��2q�$	��488������ *TitleBackground+._��� $
��Left=�$	��660��$	��480��$
 Left=�$��3q�  ScrolltabStyle#��[ . �� SelectedIndex. �� *ObjectIndex+���� *	TabValues+$ PropertyChanged=�$ TwoWay=�