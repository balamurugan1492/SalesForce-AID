   M S B A M L   `   `   `  ����I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=null?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers  5	   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5
   	   -    'StatTickerFive.Views.ObjectConfigWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Ge9http://schemas.microsoft.com/expression/2010/interactions     5   	   5c-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   0local$clr-namespace:StatTickerFive.Helpers  5	   	   XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  5
   	   -   5        $��AdminConfig��5   &     � Title$  ObjectConfigurations��6	   $
��Auto��69   $
��Auto��6G    � SizeToContent$ WidthAndHeight=�6T    � Topmost .   � WindowStyle$
 None=�6�    � WindowStartupLocation$ CenterScreen=�6�    � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��5   	   ��5   �   �� *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ 	Activated��6   .  (System.Windows.Interactivity.TriggerBase	   Actions.	 5      6  0System.Windows.Interactivity.InvokeCommandAction  
   Command
 6$   �� *ActivatedCommand+6K   5        5   
   $ Deactivated��6   .	 5        
 6$   �� *DeactivateCommand+6L   5        5   
   $ MouseLeftButtonDown��6   .	 5        
 6$   �� *	DragCmd+   CommandParameter 6@   ��  �� ElementName$ AdminConfig�� �� Path$ Title�6�   5        5   
   $ Loaded��6   .	 5        
 6$   �� *WinLoadCommand+6I   5       � 	Resources 5       �� MergedDictionaries �� Source    booleanToVisibilityConverter,   &StatTickerFive.Helpers.LengthConverter  dataGridLengthConverter�� 5   
    5      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�5       %:   &	        &	      �� 5"      6R     5#      6F   5$      .��5&      �� $
��0.89q�6    �� CornerRadius$ 2��6$   $
��Auto��6   $	��705��6�   
���������65   �� *BorderBrush+ J� BitmapEffect 6Y   �� *ShadowEffect+._�5'   
   � $	��625��6   $	��705��6    � ColumnDefinitions 5(      �� 5)      $��30 �6#   �� 5*      $��* �6#    � RowDefinitions 5,      �� 5-      $��27 �6    6.   �� 5.      $��* �6    6-   .+�50      �� $ 2,2,0,0��6   $��27��6S   $��2��6_   ��6,   �� *TitleBackground+._�51      �  52      �� 53      $
��Auto �6+   �� 54      $��* �6+   �� 55      $��25 �6+   .+�57      �� $��0��6    $��8,3,0,0q�60   $��Object Configuration��6A   $
��Left=�6^   $��Stretch=�6y   $
��Bold"�6�   $��14(�6�   ��6�   �� *TitleForeground+6�   �� 58      $��1��6    $��0,3,5,0q�60   $��Right=�6A   $
��Bold"�6~   $��14(�6�   ��6]   �� *ApplicationName+��6�   �� *TitleForeground+�� 59      -   5        $��btnExit��59   -   $��2��6   $��Center=�6<   $��20��6Y   $��20��6e     AdminCloseButton#��[ 6p   $��2q�6�   ��6�   �� *ConfigCancelCmd+��6�   �� $ AdminConfig��$ Title�5:      5;      �� 5<      -   5        $��AdminTabControl��5<   H   $��1��6   $��2��6&   $	��670��6;   $	��616��6_    �� TabStripPlacement$	 Top=�6k   .@�5=      �� -   5        $��ApplicationTabItem��5=   >   $ ��Application configurations��6   $
��Left=�6X   $
��Auto��6s   .��5>      �� 
�������$��1q�60   $��2q�6D   ���#D6D7D6$	��637��6e   $	��610��6r   ._�5?      � $
��Auto��6   $
��Auto��6-    5@      �� 5A   "   $��40 �60   6>   �� 5B   "   $	��283 �60   6?   �� 5C   "   $	��283 �60   6?   �� 5D   "   $��* �60   6=    5F      �� 5G   "   $	��300 �63   6A   �� 5H   "   $��* �63   6?   .+�5K      �� ���#D6D7D6
�������$��1q�6N   $��0��6b   $��0��6o   $��2��6   $��36��6�   $
��Auto��6�   $��2q�6�   ._�5L   "   � $	��595��6,   $��0,5,0,0q�68   .+�5M   &   �  5N   *   �� 5O   .   $	��130 �6?   6M   �� 5P   .   $��* �6?   6K   .+�5R   *   �� $��Select an Object Type��64   $��0��6Q   $
��auto��6a   $��Center=�6n   $
��Left=�6�   $��0q�6�   6�   �� 5S   *   -   5        $��cmbObjectType��5S   3    �� DisplayMemberPath$
 Text��6H   $��1��6a   $
��Left=�6q   $	��150��6�   ��6�   �� *ObjectTypes+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedIndex 6�   �� *ObjectIndex+ 5T   .     5U   2   $ SelectionChanged��6A   .	 5V   6     
 6L   �� *ObjectTypeChangedCommand+ 6y   �� $ cmbObjectType��$ SelectedItem�6�   5W   3   5Y   +   5Z   '   5[   #   5\      �� 5^      ���#D6D7D6
�������$	��Top=�6N   $��1q�6f   $��1��6z   $��0��6�   $��2��6�   $	��278��6�   $
��Auto��6�   $��2q�6�   ._�5_   "   �� $	��0.5q�6Q   $��1��6g   $��0��6t   $��2��6�   $��2q�6�   $
��Auto��6�   ��6)   �� *TitleBackground+._�5`   &   � $
��Auto��60   .+�5a   *   &   System.Windows.Controls.DataGrid  -   5        $��StatisticsGridView��5a   Q      CanUserReorderColumns .     HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6
�������   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$ Column=�6�       AutoGenerateColumns  .  !   CanUserAddRows! .    
CustomGrid#��[ 6�  $	��597��6�  $	��272��6�  $"   HorizontalScrollBarVisibility$" Disabled=�6�  "#   VerticalScrollBarVisibility$# Visible=�6(  $   
IsReadOnly$ .  ��6  �� *ConfiguredStatistics+% �� NotifyOnTargetUpdated% . $ PropertyChanged=�$ TwoWay=�&   Columns& 5b   .   4  .System.Windows.Controls.DataGridTemplateColumn  5c   2   ,  &System.Windows.Controls.DataGridColumn'   Width5	  /System.Windows.Controls.DataGridLengthConverter$' 25	 6I   (   CellTemplate( 5d   6   �� 5e   :   .R�5f   >   �� $��chkbox��6G   $��5q�6U   $��20��6  ) d� 	IsChecked) 6`   �� *isGridChecked+$ TwoWay=�$ PropertyChanged=�{�6�   �� *IsCheckBoxEnabled+$ TwoWay=�$ PropertyChanged=���6  �� *" DataContext.ObjectCheckedCommand+* �� RelativeSource* �� + �� AncestorType$+ DataGridL���6�  �� 5g   ?   5h   ;   5j   3     5l   2   $	' 273	 6I   ,   Header$, Section/Statistics Name��6U   ( 5m   6   �� 5n   :   .R�5o   >   �� $��lblStatName��6H   - �� Padding$- 5,0,0,0q�6[   ��6m   �� *SectionName+$ TwoWay=�$ PropertyChanged=�. � ToolTip. 5p   B   �� 5q      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5s      �� $��SectionNameAvail��6$   $��0��6�   $- 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6<   �� *SectionName+$ TwoWay=�$ PropertyChanged=�6�   5u      5w   ?   5x   ;   5z   3     5|   2   $	' 279	 6I   $, Description��6U   ( 5}   6   �� 5~   :   .R�5   >   �� $��lblDisplayName��6H   $- 5,0,0,0q�6^   ��6p   �� *DisplayName+$ TwoWay=�$ PropertyChanged=�. 5�   B   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�      �� $��DisplayNameAvail��6$   $��0��6�   $- 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6<   �� *DisplayName+$ TwoWay=�$ PropertyChanged=�6�   5�      5�   ?   5�   ;   5�   3   5�   +   5�   '   5�   #   5�      �� 5�      ���#D6D7D6$	��Top=�6;   
�������$��1q�6f   $��2��6z   $��0��6�   $��2��6�   $	��278��6�   $
��Auto��6�   $��2q�6�   ._�5�   "   �� $	��0.5q�6Q   $��2��6g   $��0��6t   $��2��6�   $��2q�6�   $
��Auto��6�   ��6)   �� *TitleBackground+._�5�   &   � $
��Auto��60   .+�5�   *   � -   5        $��ProxyElement��5�   ;   $y�	Collapsed=�6Q     5�   *   -   5        $��ObjectsGridView��5�   R    .   �#D6D7D6 �#D6D7D6
������� .   .  $ Column=�6�     .  ! .  #��[ 6�  $	��597��6�  $	��272��6�  $" Disabled=�6�  $# Visible=�6  ��6  �� *SelectedObject+$ TwoWay=�$ PropertyChanged=�& 5�   .     5�   2   $' 25	 6I   ( 5�   6   �� 5�   :   .R�5�   >   �� $��chkbox��6G   $��5q�6U   $��20��6�   ) 6`   �� *IsObjectChecked+$ TwoWay=�$ PropertyChanged=�6�   5�   ;   5�   3     5�   2   /   
Visibility$/ Visible=�6�   , 6I   �� *DataContext.ObjectHeaderName+0 �� Source0 %
  System.Windows.Markup.Reference
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�' 6�   �� *#!DataContext.ObjectNameColumnWidth+0 
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�1 �� 	Converter#1 [ ( 5�   6   �� 5�   :   .R�5�   >   �� $��lblStatName��6Z   $- 5,0,0,0q�6H   ��6m   �� *
ObjectName+$ TwoWay=�$ PropertyChanged=�. 5�   B   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�      �� $��SectionNameAvail��6$   $��0��6�   $- 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6<   �� *
ObjectName+$ TwoWay=�$ PropertyChanged=�6�   5�      5�   ?   5�   ;   5�   3     5�   2   $	' 280	 6t  , 6I   �� *DataContext.SwitchtHeaderName+0 
  *ProxyElement+$ TwoWay=�$ PropertyChanged=�/ 6�   �� *20DataContext.ObjectSwitchNameGridColumnVisibility+0 
  *ProxyElement+#1 [  ( 5�   6   �� 5�   :   .R�5�   >   �� $��lblSwitchName��6Z   $- 5,0,0,0q�6H   ��6o   �� *ObjectSwitchName+$ TwoWay=�$ PropertyChanged=�. 5�   B   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�      �� $��SectionNameAvail��6$   $��0��6�   $- 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6<   �� *ObjectSwitchName+$ TwoWay=�$ PropertyChanged=�6�   5�      5�   ?   5�   ;   5�   3   5�   +   5�   '   5�   #   5�      �� 5�      $��3��6%   $��0��62     NormalButton#��[ 6B   $
��Save��6i   $��23��6x   $��75��6�   $��Right=�6�   $��0,0,2,0q�6�   ��6�   �� *SaveObjectsCommand+��6�   �� $ cmbObjectType��$ SelectedValue�. 5�   "   �� 5�   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5�   *   �� -	   5        $��SaveSettings��5�   4   $��Save Objects��6H   $��0��6\   $- 	10,5,10,0q�6i   $
��Bold"�6}   $��Calibri)�6�   6�   5�   '   5�      �� 5�      $��3��6%   $��1��62   #��[ 6B   $
��Skip��6i   $��23��6x   $��75��6�   $
��Left=�6�   $��0,0,2,0q�6�   ��6�   �� *ConfigCancelCmd+��6�   �� $ AdminConfig��$ Title�. 5�   "   �� 5�   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5�   *   �� -
   5        $��SkipSettings��5�   4   $��Skip Settings��6H   $��0��6]   $- 	10,5,10,0q�6j   $
��Bold"�6~   $��Calibri)�6�   6�   5�   '   5�      5�      5�      5�      5�      5�      5�      5�      