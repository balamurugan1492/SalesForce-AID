   M S B A M L   `   `   `  ���� I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=null?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers  c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity -    'StatTickerFive.Views.ObjectConfigWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Ge9http://schemas.microsoft.com/expression/2010/interactions     5c-http://schemas.microsoft.com/wpf/2008/toolkit  0local$clr-namespace:StatTickerFive.Helpers  XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  -   $��AdminConfig��  � Title$  ObjectConfigurations��$
��Auto��$
��Auto�� � SizeToContent$ WidthAndHeight=� � Topmost .   � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png������ *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ 	Activated��.  (System.Windows.Interactivity.TriggerBase	   Actions.	 6  0System.Windows.Interactivity.InvokeCommandAction  
   Command
 �� *ActivatedCommand+  $ Deactivated��.	   
 �� *DeactivateCommand+  $ MouseLeftButtonDown��.	   
 �� *	DragCmd+   CommandParameter ��  �� ElementName$ AdminConfig�� �� Path$ Title�  $ Loaded��.	   
 �� *WinLoadCommand+ � 	Resources  �� MergedDictionaries �� Source    booleanToVisibilityConverter,   &StatTickerFive.Helpers.LengthConverter  dataGridLengthConverter��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%   &	        &	      ��   .���� $
��0.89q� �� CornerRadius$ 2��$
��Auto��$	��705��
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $	��625��$	��705�� � ColumnDefinitions �� $��30 ��� $��* � � RowDefinitions �� $��27 ��� $��* �.+��� $ 2,2,0,0��$��27��$��2������ *TitleBackground+._��  �� $
��Auto ��� $��* ��� $��25 �.+��� $��0��$��8,3,0,0q�$��Object Configuration��$
��Left=�$��Stretch=�$
��Bold"�$��14(����� *TitleForeground+�� $��1��$��0,3,5,0q�$��Right=�$
��Bold"�$��14(����� *ApplicationName+���� *TitleForeground+�� -   $��btnExit��$��2��$��Center=�$��20��$��20��  AdminCloseButton#��[ $��2q����� *ConfigCancelCmd+���� $ AdminConfig��$ Title��� -   $��AdminTabControl��$��1��$��2��$	��670��$	��616�� �� TabStripPlacement$	 Top=�.@��� -   $��ApplicationTabItem��$ ��Application configurations��$
��Left=�$
��Auto��.���� 
�������$��1q�$��2q����#D6D7D6$	��637��$	��610��._�� $
��Auto��$
��Auto�� �� $��40 ��� $	��283 ��� $	��283 ��� $��* � �� $	��300 ��� $��* �.+��� ���#D6D7D6
�������$��1q�$��0��$��0��$��2��$��36��$
��Auto��$��2q�._�� $	��595��$��0,5,0,0q�.+��  �� $	��130 ��� $��* �.+��� $��Select an Object Type��$��0��$
��auto��$��Center=�$
��Left=�$��0q��� -   $��cmbObjectType�� �� DisplayMemberPath$
 Text��$��1��$
��Left=�$	��150������ *ObjectTypes+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedIndex �� *ObjectIndex+   $ SelectionChanged��.	   
 �� *ObjectTypeChangedCommand+ �� $ cmbObjectType��$ SelectedItem��� ���#D6D7D6
�������$	��Top=�$��1q�$��1��$��0��$��2��$	��278��$
��Auto��$��2q�._��� $	��0.5q�$��1��$��0��$��2��$��2q�$
��Auto������ *TitleBackground+._�� $
��Auto��.+�&   System.Windows.Controls.DataGrid  -   $��StatisticsGridView��   CanUserReorderColumns .     HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6
�������   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$ Column=�    AutoGenerateColumns  .  !   CanUserAddRows! .    
CustomGrid#��[ $	��597��$	��272��$"   HorizontalScrollBarVisibility$" Disabled=�"#   VerticalScrollBarVisibility$# Visible=�$   
IsReadOnly$ .  ���� *ConfiguredStatistics+% �� NotifyOnTargetUpdated% . $ PropertyChanged=�$ TwoWay=�&   Columns& 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn'   Width5	  /System.Windows.Controls.DataGridLengthConverter$' 25	 (   CellTemplate( �� .R��� $��chkbox��$��5q�$��20��) d� 	IsChecked) �� *isGridChecked+$ TwoWay=�$ PropertyChanged=�{��� *IsCheckBoxEnabled+$ TwoWay=�$ PropertyChanged=����� *" DataContext.ObjectCheckedCommand+* �� RelativeSource* �� + �� AncestorType$+ DataGridL�����   $	' 273	 ,   Header$, Section/Statistics Name��( �� .R��� $��lblStatName��- �� Padding$- 5,0,0,0q����� *SectionName+$ TwoWay=�$ PropertyChanged=�. � ToolTip. �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$- 	10,5,10,0q�$
��Bold"�$��Calibri)����� *SectionName+$ TwoWay=�$ PropertyChanged=�  $	' 279	 $, Description��( �� .R��� $��lblDisplayName��$- 5,0,0,0q����� *DisplayName+$ TwoWay=�$ PropertyChanged=�. �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��DisplayNameAvail��$��0��$- 	10,5,10,0q�$
��Bold"�$��Calibri)����� *DisplayName+$ TwoWay=�$ PropertyChanged=��� ���#D6D7D6$	��Top=�
�������$��1q�$��2��$��0��$��2��$	��278��$
��Auto��$��2q�._��� $	��0.5q�$��2��$��0��$��2��$��2q�$
��Auto������ *TitleBackground+._�� $
��Auto��.+�� -   $��ProxyElement��$y�	Collapsed=�  -   $��ObjectsGridView�� .   �#D6D7D6 �#D6D7D6
������� .   .  $ Column=�  .  ! .  #��[ $	��597��$	��272��$" Disabled=�$# Visible=����� *SelectedObject+$ TwoWay=�$ PropertyChanged=�&   $' 25	 ( �� .R��� $��chkbox��$��5q�$��20��) �� *IsObjectChecked+$ TwoWay=�$ PropertyChanged=�  /   
Visibility$/ Visible=�, �� *DataContext.ObjectHeaderName+0 �� Source0 %
  System.Windows.Markup.Reference
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�' �� *#!DataContext.ObjectNameColumnWidth+0 
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�1 �� 	Converter#1 [ ( �� .R��� $��lblStatName��$- 5,0,0,0q����� *
ObjectName+$ TwoWay=�$ PropertyChanged=�. �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$- 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
ObjectName+$ TwoWay=�$ PropertyChanged=�  $	' 280	 , �� *DataContext.SwitchtHeaderName+0 
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�/ �� *20DataContext.ObjectSwitchNameGridColumnVisibility+0 
  *ProxyElement+#1 [  ( �� .R��� $��lblSwitchName��$- 5,0,0,0q����� *ObjectSwitchName+$ TwoWay=�$ PropertyChanged=�. �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$- 	10,5,10,0q�$
��Bold"�$��Calibri)����� *ObjectSwitchName+$ TwoWay=�$ PropertyChanged=��� $��3��$��0��  NormalButton#��[ $
��Save��$��23��$��75��$��Right=�$��0,0,2,0q����� *SaveObjectsCommand+���� $ cmbObjectType��$ SelectedValue�. �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -	   $��SaveSettings��$��Save Objects��$��0��$- 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��3��$��1��#��[ $
��Skip��$��23��$��75��$
��Left=�$��0,0,2,0q����� *ConfigCancelCmd+���� $ AdminConfig��$ Title�. �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -
   $��SkipSettings��$��Skip Settings��$��0��$- 	10,5,10,0q�$
��Bold"�$��Calibri)�