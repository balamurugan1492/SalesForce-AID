   M S B A M L   `   `   `  ���� I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=null?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers  c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity -    'StatTickerFive.Views.ObjectConfigWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Ge9http://schemas.microsoft.com/expression/2010/interactions     5c-http://schemas.microsoft.com/wpf/2008/toolkit  0local$clr-namespace:StatTickerFive.Helpers  XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  -   $��QueueConfig��  � Title$  ObjectConfigurations��$
��Auto��$
��Auto�� � SizeToContent$ WidthAndHeight=� � 
ResizeMode$ NoResize=� � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png�� � ShowInTaskbar .   � Topmost �� *	IsTopmost+���� *BackgroundColor+.  (System.Windows.Interactivity.Interaction	   Triggers	 /  )System.Windows.Interactivity.EventTrigger  
   	EventName$
 	Activated��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command �� *ActivatedCommand+  $
 Deactivated��.    �� *DeactivateCommand+  $
 MouseLeftButtonDown��.    �� *	DragCmd+   CommandParameter ��  �� ElementName$ QueueConfig�� �� Path$ Title�  $
 Loaded��.    �� *WinLoadCommand+ � 	Resources  �� MergedDictionaries �� Source    booleanToVisibilityConverter,   &StatTickerFive.Helpers.LengthConverter  dataGridLengthConverter��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%   &	        &	      ��   .���� $
��0.89q� �� CornerRadius$ 2��$
��Auto��$
��Auto��
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $	��525��$
��Auto�� � ColumnDefinitions �� $��30 ��� $��* � � RowDefinitions �� $��27 ��� $��* �.+��� $ 2,2,0,0��$��27��$��2������ *TitleBackground+._��  �� $
��Auto ��� $��* ��� $��20 �.+��� $��0��$��8,3,0,0q�$��Queue Selection��$
��Left=�$��Stretch=�$
��Bold"�$��14(����� *TitleForeground+�� -   $��btnExit��$��2��$��Center=�$��20��$��20��  AdminCloseButton#��[ $��2q����� *ConfigCancelCmd+���� $ QueueConfig��$ Title��� -   $��AdminTabControl��$��1��$��2��$
��Auto��$	��516�� �� TabStripPlacement$	 Top=�$��3q�.@��� -   $��ApplicationTabItem��$
��Left=�$
��Auto��.���� 
�������$��1q�$��2q����#D6D7D6$
��Auto��$	��510��._�� $
��Auto��$
��Auto�� �� $��0 ��� ���� *StatisticsRowHeight+�� $	��283 ��� $��* � �� $	��200 ��� $��* �.+��� ���#D6D7D6
�������$��1q�$��0��$��0��$��2��$��36��$
��Auto��$��2q�._�� $	��495��$��0,5,0,0q�.+��  �� $	��130 ��� $��* �.+��� $��Select an Object Type��$��0��$
��auto��$��Center=�$
��Left=�$��0q��� -   $��cmbObjectType�� �� DisplayMemberPath$
 Text��$��1��$
��Left=�$	��150������ *ObjectTypes+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedIndex �� *ObjectIndex+	   $
 SelectionChanged��.    �� *ObjectTypeChangedCommand+ �� $ cmbObjectType��$ SelectedItem��� ���#D6D7D6
�������$	��Top=�$��1q�$��1��$��0��$��2��$	��278��$
��Auto��$��2q�._��� $	��0.5q�$��1��$��0��$��2��$��2q�$
��Auto������ *TitleBackground+._�� $
��Auto��.+�&   System.Windows.Controls.DataGrid  -   $��StatisticsGridView��   CanUserReorderColumns .     HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6
�������   CanUserResizeColumns .      CanUserResizeRows  .  !   HeadersVisibility$! Column=�"   AutoGenerateColumns" .  #   CanUserAddRows# .    
CustomGrid#��[ $	��497��$	��272��$$   HorizontalScrollBarVisibility$$ Disabled=�"%   VerticalScrollBarVisibility$% Visible=�&   
IsReadOnly& .  ���� *ConfiguredStatistics+'   Columns' 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn(   Width5	  /System.Windows.Controls.DataGridLengthConverter$( 25	 )   CellTemplate) �� .R��� $��chkbox��$��5q�$��20��* d� 	IsChecked* �� *isGridChecked+$ TwoWay=�$ PropertyChanged=�{��� *IsCheckBoxEnabled+$ TwoWay=�$ PropertyChanged=����� *" DataContext.ObjectCheckedCommand++ �� RelativeSource+ �� , �� AncestorType$, DataGridL�  $	( 223	 -   Header$- Section/Statistics Name��) �� .R��� $��lblStatName��. �� Padding$. 5,0,0,0q����� *SectionName+$ TwoWay=�$ PropertyChanged=�/ � ToolTip/ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$. 	10,5,10,0q�$
��Bold"�$��Calibri)����� *SectionName+$ TwoWay=�$ PropertyChanged=�  $	( 229	 $- Description��) �� .R��� $��lblDisplayName��$. 5,0,0,0q����� *DisplayName+$ TwoWay=�$ PropertyChanged=�/ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��DisplayNameAvail��$��0��$. 	10,5,10,0q�$
��Bold"�$��Calibri)����� *DisplayName+$ TwoWay=�$ PropertyChanged=��� ���#D6D7D6$	��Top=�
�������$��1q�$��2��$��0��$��2��$	��278��$
��Auto��$��2q�._��� $	��0.5q�$��2��$��0��$��2��$��2q�$
��Auto������ *TitleBackground+._�� $
��Auto��.+�� -   $��ProxyElement��$y�	Collapsed=�  -   $��ObjectsGridView�� .   �#D6D7D6 �#D6D7D6
������� .    .  $! Column=�" .  # .  #��[ $	��497��$	��272��$$ Disabled=�$% Visible=����� *SelectedObject+$ TwoWay=�$ PropertyChanged=�'   - �� $��2q�* �� + �� ",  $&  DataContext.IsDeselectAllChecked����� *DataContext.DeselectUncheck++ �� $, DataGridL����� ) �� .R��� $��chkbox��$��5q�$��20��* �� *IsObjectChecked+$ TwoWay=�$ PropertyChanged=����� *#!DataContext.CheckedObjectsCommand++ �� $, DataGridL�����   0   
Visibility$0 Visible=�$	( 150	 - �� *DataContext.ObjectHeaderName+1 �� Source1 %
  System.Windows.Markup.Reference
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�) �� .R��� $��lblStatName��$. 5,0,0,0q����� *
ObjectName+$ TwoWay=�$ PropertyChanged=�/ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$. 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
ObjectName+$ TwoWay=�$ PropertyChanged=�  $0 Visible=�$	( 150	 - �� *DataContext.ObjectType+1 
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�) �� .R��� $��lblObjectName��$. 5,0,0,0q����� *
TypeObject+$ TwoWay=�$ PropertyChanged=�/ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��ObjType��$��0��$. 	10,5,10,0q�$
��Bold"�$��Calibri)����� *
TypeObject+$ TwoWay=�$ PropertyChanged=�  $	( 146	 - �� *DataContext.SwitchtHeaderName+1 
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�0 �� *20DataContext.ObjectSwitchNameGridColumnVisibility+1 
  *ProxyElement+2 �� 	Converter#2 [  ) �� .R��� $��lblSwitchName��$. 5,0,0,0q����� *ObjectSwitchName+$ TwoWay=�$ PropertyChanged=�/ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$. 	10,5,10,0q�$
��Bold"�$��Calibri)����� *ObjectSwitchName+$ TwoWay=�$ PropertyChanged=�� $��3��$��1��$��Right=�.+��� $��1��  NormalButton#��[ $
��Save��$��23��$��75��$��Right=�$��2,2,5,2q����� *SaveObjectsCommand+���� $ cmbObjectType��$ SelectedValue�/ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -	   $��SaveSettings��$��Save Objects��$��0��$. 	10,5,10,0q�$
��Bold"�$��Calibri)�� $��3��$��0��$
��Left=�.+��� $��0��$
��Left=�$��3,0,0,0q�
���   �$
��Bold"����� *SelectedCount+$ TwoWay=�$ PropertyChanged=�