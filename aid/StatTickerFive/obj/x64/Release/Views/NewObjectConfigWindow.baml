   M S B A M L   `   `   `  ���� I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=nullE'clr-namespace:StatTickerFive.ConvertersStatTickerFive.Converters  c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 0    *StatTickerFive.Views.NewObjectConfigWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Ge9http://schemas.microsoft.com/expression/2010/interactions     5c-http://schemas.microsoft.com/wpf/2008/toolkit  3local'clr-namespace:StatTickerFive.Converters  XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  -   $��NewObjectConfiguration��  � Title$  NewObjectConfig��$
��Auto��$
��Auto�� � SizeToContent$ WidthAndHeight=� � Topmost .   � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png������ *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ 	Activated��.  (System.Windows.Interactivity.TriggerBase	   Actions.	 6  0System.Windows.Interactivity.InvokeCommandAction  
   Command
 �� *ActivatedCommand+  $ Deactivated��.	   
 �� *DeactivateCommand+  $ MouseLeftButtonDown��.	   
 �� *	DragCmd+   CommandParameter ��  �� ElementName$ NewObjectConfiguration�� �� Path$ Title�  $ Loaded��.	   
 �� *WinLoadCommand+ � 	Resources  �� MergedDictionaries �� Source��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%    .���� $
��0.89q� �� CornerRadius$ 2��$	��480��$	��200��
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $	��479��$	��198�� � RowDefinitions �� $��27 ��� $��27 ��� $��* �.+��� $ 2,2,0,0��$��27��$��0������ *TitleBackground+._��  � ColumnDefinitions �� $
��Auto ��� $��* ��� $��25 �.+��� $��0��$��8,3,0,0q�$��Select Object��$
��Left=�$��Stretch=�$
��Bold"�$��14(����� *TitleForeground+�� -   $��btnExit��$��2��$��Center=�$��20��$��20��   AdminCloseButton#��[  $��2q����� *ConfigCancelCmd+���� $ NewObjectConfiguration��$ Title��� $��1��$��5,0,0,0q�$��20��$
��Left=�$��20��
������   ImageButton#��[  � ToolTip$
 Save������ *	SaveCmd+���� $ NewObjectConfiguration��$ Title�.���� $/��)/StatTickerFive;component/Images/save.png��$��19��$��20���� $��2��$	��Top=�
�������$��1q�$��2q�$	��140������ *TitleBackground+._��� $	��Top=�
�������$��1q�$��2q�$	��135������ *TitleBackground+._��  �� $��25 ��� $��* � �� $	��225 ��� $��* �.+��� $��0��$��0��$��1,1,1,1q����#D6D7D6
��������� $��0��$��0��$��Object Name��$��Center=�$
��Bold"�$��Center=�$��12(�$��Calibri)�
���   ��� $��0��$��1��$��1,1,1,1q����#D6D7D6
��������� $��0��$��1��$��Switch Name��$��Center=�$
��Bold"�$��Center=�$��12(�$��Calibri)�
���   ��� $��1��$��0��$��2��.��� .+�� -   $��ProxyElement��$y�	Collapsed=�&   System.Windows.Controls.DataGrid  -   $��ObjectsGridView��   CanUserReorderColumns .     HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6
�������   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$
 None=�   AutoGenerateColumns .     CanUserAddRows .    
CustomGrid#��[ $	��465��$   HorizontalScrollBarVisibility$ Disabled=�"   VerticalScrollBarVisibility$ Hidden=����� *SelectedObject+  �� Mode$  TwoWay=�! �� UpdateSourceTrigger$! PropertyChanged=�"   Columns" 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn#   Width5  /System.Windows.Controls.DataGridLengthConverter$# 25 $   CellTemplate$ �� .R��� $��chkbox��$��5q�$��20������ *DataContext.NewObjectSelected+% �� RelativeSource% �� & �� AncestorType$& DataGridL����� ' d� 	IsChecked' �� *IsObjectChecked+$  TwoWay=�$! PropertyChanged=�  (   Header$( ACDQueue��$	# 200 $ �� .R��� $��lblStatName��) �� Padding$) 5,0,0,0q����� *
ObjectName+$  TwoWay=�$! PropertyChanged=� �� *
ObjectName+$  TwoWay=�$! PropertyChanged=�  $( Switch Name��$	# 223 $ �� .R��� $��lblSwitchName��$) 5,0,0,0q����� *ObjectSwitchName+$  TwoWay=�$! PropertyChanged=� �� *ObjectSwitchName+$  TwoWay=�$! PropertyChanged=�