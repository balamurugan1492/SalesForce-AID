   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	   I EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=nullE'clr-namespace:StatTickerFive.ConvertersStatTickerFive.Converters 5	   	   ?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers 5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	   ,   &StatTickerFive.Views.AdminConfigWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Ge9http://schemas.microsoft.com/expression/2010/interactions     5   	   5c-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   Dxcd:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit  5   	   2conv'clr-namespace:StatTickerFive.Converters  5	   	   Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4UxwtkJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  5
   
   0local$clr-namespace:StatTickerFive.Helpers  5   	   XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  5   	   -   5        $��AdminConfig��5   %     � Title$  AdminConfigurations��6	   $
��Auto��68    � 
ResizeMode$ NoResize=�6F   $
��Auto��6\    � SizeToContent$ WidthAndHeight=�6i    � Topmost .   � WindowStyle$
 None=�6�    � WindowStartupLocation$ CenterScreen=�6�    � AllowsTransparency .   � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��5   	   ��5   �   �� *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
   	   	EventName$	 	Activated��6   .  (System.Windows.Interactivity.TriggerBase
   Actions.
 5      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *ActivatedCommand+6K   5        5   
   $	 Deactivated��6   .
 5         6$   �� *DeactivateCommand+6L   5        5   
   $	 MouseLeftButtonDown��6   .
 5         6$   �� *	DragCmd+   CommandParameter 6@   ��  �� ElementName$ AdminConfig�� �� Path$ Title�6�   5        5   
   $	 Loaded��6   .
 5         6$   �� *WinLoadCommand+6I   5       � 	Resources 5       �� MergedDictionaries �� Source.  (StatTickerFive.Converters.MultiConverter   MultiValueConverters�� 5    
    5!      �� 5"      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�5#      %   &	          5%      6"   5&      .��5)      �� $
��0.89q�6    �� CornerRadius$ 2��6$   $
��Auto��6   $	��705��6�   
���������65   �� *BorderBrush+ J� BitmapEffect 6Y   �� *ShadowEffect+._�5*   
   � $
��Auto��6   $	��705��6    � RowDefinitions 5+      �� 5,      $��28 �6    6.   �� 5-      $��* �6    6-    � ColumnDefinitions 5/      �� 50      $	��525 �6#   61   �� 51      ��6#   �� *	GridWidth+6A   .+�53      �� $��0��6   $��30��6I   $��0��6U   $
��Auto��6�   ��6"   �� *TitleBackground+��6e   �� *	TitleSpan+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�._�54      �  55      �� 56      $
��Auto �6+   �� 57      $��* �6+   �� 58      $��25 �6+   .+�5;      �� $��0��6    $��8,3,0,0q�60   $��Admin Configuration��6A   $
��Left=�6]   $��Stretch=�6x   $
��Bold"�6�   $��14(�6�   ��6�   �� *TitleForeground+6�   �� 5<      $��1��6    $��0,3,5,0q�60   $��Right=�6A   $
��Bold"�6~   $��14(�6�   ��6]   �� *ApplicationName+��6�   �� *TitleForeground+�� 5=      -   5        $��btnExit��5=   -   $��2��6   $��Center=�6<   $��20��6Y   $��20��6e     AdminCloseButton#��[ 6p   $��2q�6�   ��6�   �� *ConfigSkipCmd+5?      5@      �� 5A      $��1��6   
�������$��1q�65   ���#D6D7D6$	��669��6_   $	��518��6l   ._�5B      �� -   5        $��AdminTabControl��5B   5   $��3q�6   $	��661��6(   $	��510��6L    �� TabStripPlacement$	 Top=�6X   .@�5C      �� -   5        $��ApplicationTabItem��5C   3   $��Application��6   $
��Left=�6M   .��5D      � $	��628��6   $	��504��6,    5E      �� 5F   "   $	��600 �60   6?   �� 5G   "   $��30 �60   6>    5J      �� 5K   "   $	��255 �63   6A   �� 5L   "   $��* �63   6?   .+�5P      �� $	��0.5q�6M   $��0��6c   $��0��6p   $��3��6�   $��0q�6�   ��6%   �� *TitleBackground+._�5Q   "   �  5R   &   �� 5S   *   $��25 �68   6F   �� 5T   *   $��* �68   6E    5V   &   �� 5W   *   $��25 �6;   6H   �� 5X   *   $	��190 �6;   6I   �� 5Y   *   $��70 �6;   6H   �� 5Z   *   $	��195 �6;   6I   �� 5[   *   $��19 �6;   6H   .+�5]   &   �� $��0��6-   $��0��6=   $��1,1,1,1q�6J   $��0,0,0,0q�6d   ���#D6D7D6
�������6�   �� 5^   &   $��1��6-   $��0��6=   $��1,1,1,1q�6J   $��0,0,0,0q�6d   ���#D6D7D6
�������6�   �� 5_   &   $��2��6-   $��0��6=   $��1,1,1,1q�6J   $��0,0,0,0q�6d   ���#D6D7D6
�������6�   �� 5`   &   $��3��6-   $��0��6=   $��1,1,1,1q�6J   $��0,0,0,0q�6d   ���#D6D7D6
�������6�   �� 5a   &   $��0��6,   $��1��69   $��Section/Statistics Name��6I   $��Center=�6k   $
��Bold"�6�   $��Center=�6�   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5b   &   $��0��6,   $��2��69   $��
Properties��6I   $��Center=�6^   $��Center=�6y   $
��Bold"�6�   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5c   &   $��0��6,   $��3��69   $��Description��6I   $��Center=�6_   $��Center=�6z   $
��Bold"�6�   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5e   &   $��1��63   $��0��6@   $��5��6P   .��5f   *   � $	��508��64   .+�5g   .   &   System.Windows.Controls.DataGrid  -   5        $"��ConfiguredStatisticsGridView��5g   8      HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$
 None=�6�      AutoGenerateColumns .     CanUserAddRows .    
CustomGrid#��[ 6�  $	��490��6�  $    HorizontalScrollBarVisibility$  Disabled=�6�  "!   VerticalScrollBarVisibility$! Disabled=�6  "   RowBackground" 6�   �� *ExistingColor+$ PropertyChanged=�$ TwoWay=���6I  �� *ConfiguredStatistics+#   Columns# 5h   2   4  .System.Windows.Controls.DataGridTemplateColumn  5i   6   ,  &System.Windows.Controls.DataGridColumn$   Width5	  /System.Windows.Controls.DataGridLengthConverter$$ 25	 6M   %   CellTemplate% 5j   :   �� 5k   >   .R�5l   B   �� $��chkbox��6K   $��5q�6Y   $��20��5o   /   & d� 	IsChecked& 5l   d   �� *isGridChecked+$ TwoWay=�$ PropertyChanged=���5m   /   �� *$"DataContext.ConfiguredStatSelected+' �� RelativeSource' �� ( �� AncestorType$( DataGridL���5n   /   �� 5o   <   5p   ?   5r   7     5t   6   $	$ 190	 6M   )   Header$) Section/Statistics Name��6Y   % 5u   :   �� 5v   >   .R�5w   B   �� $��tblConfiguredStatName��6^   * �� Padding$* 5,0,0,0q�6L   ��6{   �� *SectionName+$ TwoWay=�$ PropertyChanged=�+ � ToolTip+ 5x   F   �� 5y      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5{   N   �� $��SectionNameExist��6X   $��0��6�   $* 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6p   �� *SectionTooltip+$ TwoWay=�$ PropertyChanged=�6  5}      5   C   5�   ?   5�   7     5�   6   $$ 70	 6M   $) 
Properties��6X   % 5�   :   �� 5�   >   .R�5�   B   �� $��btnEdit��6I     NormalImageButton#��[ 6X   $��60��6�   $+ Edit Statistics��6  ��6�   �� *$"DataContext.EditExistingStatistics+' �� $( DataGridL���6�   �� .��5�   F   �� $
��Left=�6Q   , �� Orientation$, 
Horizontal=�6l   .+�5�   J   �� $3��-/StatTickerFive;component/Images/editIcon.png��6P   $
��Left=�6�   $��17��6�   $��20��6�   6�   �� 5�   J   $��Calibri)�6n   $��14(�6�   
���   ���6T   �� *
EditName++ 5�   N   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�      �� $��	EditAvail��6$   $ ��Edit Statistics Properties��65   $��0��6W   $* 	10,5,10,0q�6d   $
��Bold"�6x   $��Calibri)�6�   6�   5�      5�   K   5�   G   5�   C   - �� Triggers- 5�   B   �� 5�   F   . �� Value$
. true��6t   / �� Binding/ 6R   �� *isGridChecked+.a�5�   J   �� 0 �� 
TargetName$0 btnEdit��6f   1 �� Property1 � {�2 �� Value2 . 6{   5�   G   �� 5�   F   $. false��6t   / 6R   �� *isGridChecked+.a�5�   J   �� $0 btnEdit��6f   1 � {�2 .  6{   5�   G   5�   ?   5�   7     5�   6   $	$ 195	 6M   $) Description��6Y   % 5�   :   �� 5�   >   .R�5�   B   �� $��tblDisplayName��6^   $* 5,0,0,0q�6L   ��6t   �� *DisplayName+$ TwoWay=�$ PropertyChanged=�+ 5�   F   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�   !   �� $��DisplayNameAvail��6+   $��0��6�   $* 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6C   �� *DisplayTooltip+$ TwoWay=�$ PropertyChanged=�6�   5�      5�   C   5�   ?   5�   7   5�   /     5�   .   -   5        $��StatisticsServerGridView��5�   8    �#D6D7D6 �#D6D7D6 .   .  $
 None=�6�    .   .  #��[ 6�  $	��507��6�  $  Disabled=�6�  $! Disabled=�6�  " 6�   �� *ServerColor+$ PropertyChanged=�$ TwoWay=���6C  �� *ServerStatistics+# 5�   2     5�   6   $$ 25	 6M   % 5�   :   �� 5�   >   .R�5�   B   �� $��chkbox��6K   
3 � Tag$
3 null��6Y   $��5q�6d   $��20��5�   /   & 5�   o   �� *isGridChecked+$ TwoWay=�$ PropertyChanged=���5�   /   �� *&$DataContext.ActivatedCheckboxCommand+' �� $( DataGridL���5�   /   �� 5�   <   5�   ?   5�   7     5�   6   $	$ 190	 6M   % 5�   :   �� 5�   >   .R�5�   B   �� $��tblServerStatName��6^   $* 5,0,0,0q�6L   ��6w   �� *SectionName+$ TwoWay=�$ PropertyChanged=�+ 5�   F   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�   N   �� $��SectionNameAvail��6X   $��0��6�   $* 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6p   �� *SectionTooltip+$ TwoWay=�$ PropertyChanged=�6  5�      5�   C   5�   ?   5�   7     5�   6   $$ 70	 6M   % 5�   :   �� 5�   >   .R�5�   B   �� $��btnEdit��6I   #��[ 6X   $��60��6�   3 6�   �� *
StatName+��5�   -   �� *$"DataContext.EditExistingStatistics+' �� $( DataGridL���6�   �� .��5�   F   �� $, 
Horizontal=�6Q   .+�5�   J   �� $3��-/StatTickerFive;component/Images/editIcon.png��6P   $��17��6�   $��20��6�   6�   �� 5�   J   $��Calibri)�6n   $��14(�6�   
���   ���6T   �� *
EditName++ 5�   N   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�      �� $��EditStatAvail��6$   $ ��Edit Statistics Properties��69   $��0��6[   $* 	10,5,10,0q�6h   $
��Bold"�6|   $��Calibri)�6�   6�   5�      5�   K   5�   G   5�   C   - 5�   B   �� 5�   F   $
. true��6t   / 6R   �� *isGridChecked+.a�5�   J   �� $0 btnEdit��6f   1 � {�2 . 6{   5�   G   �� 5�   F   $. false��6t   / 6R   �� *isGridChecked+.a�5�   J   �� $0 btnEdit��6f   1 � {�2 .  6{   5�   G   5�   ?   5�   7     5�   6   $	$ 195	 6M   % 5�   :   �� 5�   >   .R�5�   B   �� $* 5,0,0,0q�6L   ��6^   �� *DisplayName+$ TwoWay=�$ PropertyChanged=�+ 5�   F   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�   N   �� $��DisplayNameAvail��6X   $��0��6�   $* 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6p   �� *DisplayTooltip+$ TwoWay=�$ PropertyChanged=�6  5�      5�   C   5�   ?   5�   7   5  /   5  +   5  '   5  #   5     �� 5     $��1��6%     NormalButton#��[ 62   $
��Save��6Y   $��23��6h   $��75��6t   $��Right=�6   $��0,5,2,2q�6�   ��6�   �� *SaveStatistics++ 5	  "   �� 5
  &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5  *   �� -   5        $��SaveSettings��5  4   $��Save Settings��6H   $��0��6]   $* 	10,5,10,0q�6j   $
��Bold"�6~   $��Calibri)�6�   6�   5  '   5     �� 5     $��1��6%   $��2��62   #��[ 6B   $
��Skip��6i   $��23��6x   $��75��6�   $
��Left=�6�   $��2,5,0,2q�6�   ��6�   �� *ConfigSkipCmd++ 5  "   �� 5  &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5  *   �� -   5        $��SkipSettings��5  4   $��Skip Settings��6H   $��0��6]   $* 	10,5,10,0q�6j   $
��Bold"�6~   $��Calibri)�6�   6�   5  '   5     5     5     5     5     �� 5     $	��395��6   $	��669��6"   $��2��6/   $��1��6?   
�������$��1q�6_   ���#D6D7D6._�5     �� $ 2,2,0,0��6   ._�5      �� $��3q�6%   $	��658��60   $	��385��6=   $	 Top=�6I   .@�5!     �� $
��Left=�6?   ��6&   �� *Title+.��5"  "   � $
��auto��6'   $	��380��65    5#  &   �� 5$  *   $��30 �68   �� 5%  *   $��* �68   .+�5(  &   �� $��0��6-   $��0��6=   $��4��6J   $	��376��6_   $
��Auto��6k   
�������$��1q�6�   ���#D6D7D6$��2q�6�   ._�5)  *   �  5*  .   �� 5+  2   $��20 �6C   �� 5,  2   $��30 �6C   .+�5.  *   �� $��0��61   $��20��6A   $��20��6�   $
��Left=�6�   
������   ImageButton#��[ 6�   {�6M   �� *IsSavebtnEnable+$ TwoWay=�$ PropertyChanged=���6
  �� *PropertySave+.��5/  2   �� $/��)/StatTickerFive;component/Images/save.png��68   $��18��6k   $��18��6w   6�    50  .     51  2   $	 
MouseEnter��6A   .
 52  6      6L   �� *IsSavebtnEnabledCommand+6z   53  3   + 55  .   �� 56  2   
����������#ADAAAD$	��0.5q�6b   $��-5q�6x   ._�58  6   �� -	   5        $��SaveProperties��58  @   $��Save Properties��6V   $��0��6m   $* 	10,5,10,0q�6z   $
��Bold"�6�   $��Calibri)�6�   6�   5:  3   ��5<  .   `� 5=  2   4 `� 	Converter#4 [  6?   .1�5>  6   �� $ txtSectionName��6>   $
 Text�6[   �� 5?  6   $ cboxSectionName��6>   $ SelectedItem�6\   �� 5@  6   $ txtStatName��6>   $
 Text�6X   5A  3   5C  +   �� 5D  .   $��1��65   $��20��6E   $��24��6Q   $
��Left=�6\   
������ #��[ 6�   ��6�   �� *CloseObjectvaluesCommand+.��5E  .   �� $/��)/StatTickerFive;component/Images/Back.png��64   $��18��6g   $��16��6s   $��2q�6~   6�   + 5F  .   �� 5G  2   
����������#ADAAAD$	��0.5q�6b   $��-5q�6x   ._�5I  6   �� -
   5        $��BackSettings��5I  @   $��Cancel Settings��6T   $��0��6k   $* 	10,5,10,0q�6x   $
��Bold"�6�   $��Calibri)�6�   6�   5K  3   5M  +   5N  +   5O  '   �� 5Q  #   $��0��6*   $��1��6:   $	��376��6G   $
��Auto��6S   
�������$��1q�6t   ���#D6D7D6$��2q�6�   ._�5R  "   � $
��Auto��6(    5S  .   �� 5T  2   $��33 �6@   �� 5U  2   $��33 �6@   �� 5V  2   $��33 �6@   �� 5W  2   $��33 �6@   �� 5X  2   $��33 �6@   �� 5Y  2   $��33 �6@   �� 5Z  2   $��33 �6@   �� 5[  2   $��33 �6@   �� 5\  2   $��33 �6@   �� 5]  2   ��6@   �� *DataGridRowHeight+ 5_  .   �� 5`  2   $	��125 �6C   �� 5a  2   $��75 �6C   �� 5b  2   $��75 �6C   �� 5c  2   $��* �6C   .+�5e  .   �� $��0��64   $��0��6D   $��Section Name��6Q   6j   �� 5f  .   -   5        $��cboxSectionName��5f  7   $��1��6N   $��0��6^   $��25��6k   $��66��6w   $��6,0,0,0q�6�   {�. ��6�   �� *FilterBorder+��6�   �� *
Sections+$ PropertyChanged=�$ TwoWay=�5 �� SelectedItem5 6  �� *SelectedSection+$ PropertyChanged=�$ TwoWay=� 5g  2     5h  6   $	 SelectionChanged��6E   .
 5i  :      6P   �� *SectionChanged+ 6s   �� $ cboxSectionName��$ SelectedItem�6�   5j  7   5l  /   1
  +Microsoft.Windows.Controls.WatermarkTextBox
  5m  *   -   5        $��txtSectionName��5m  @   $3 SectionNameWaterMarkText��6V   $��0��66  $��2��6C  $��0��6S  $	��170��6`  $��25��6l  $��2��6x  $��0,0,0,0q�6�  6 
  	Watermark6 6u   �� *SectionNameWaterMarkText+$ PropertyChanged=�$ TwoWay=���6�   �� *SectionNameWaterMarkColor+$ PropertyChanged=�$ TwoWay=���6�  �� *StatSectionName+$ PropertyChanged=�$ TwoWay=�.��6�   5n  2     5o  6   $	 GotFocus��6E   .
 5p  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5q  7     5r  2   $	 	LostFocus��6A   .
 5s  6      6L   �� *TextBoxLostFocusCommand+ 6x   �� $ txtSectionName��$	 Tag�6�   5t  3     5u  2   $	 TextChanged��6A   .
 5v  6      6L   �� *TextChangedCommand+6v   5w  3   .��5x  J   7 
  WatermarkTemplate7 5y  .   �� 5z  2   .R�5{  6   �� $��Italic$�6Y   ��6E   �� 6n   5|  3   .��5}  W   5  +   �� 5�  *   $��0��60   $��1��6@   $��Display Name��6M   6f   
  5�  *   -   5        $��txtDisplayName��5�  @   $3 DisplayNameWaterMarkText��6V   $��1��67  $��25��6D  $	��242��6P  $��3,0,0,0q�6]  $��1��6o  $��1��6  $��3��6�  6 6u   �� *DisplayNameWaterMarkText+$ PropertyChanged=�$ TwoWay=���6�   �� *DisplayNameWaterMarkColor+$ PropertyChanged=�$ TwoWay=���6�  �� *PropDisplayName+$ PropertyChanged=�$ TwoWay=�.��6�   5�  .     5�  2   $	 GotFocus��6A   .
 5�  6      6L   �� *DataGridLostFocusCommand+ 6y   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  3     5�  2   $	 	LostFocus��6A   .
 5�  6      6L   �� *TextBoxLostFocusCommand+ 6x   �� $ txtDisplayName��$	 Tag�6�   5�  3     5�  2   $	 TextChanged��6A   .
 5�  6      6L   �� *TextChangedCommand+6v   5�  3   .��5�  F   7 5�  .   �� 5�  2   .R�5�  6   �� $��Italic$�6Y   ��6E   �� 6n   5�  3   .��5�  W   5�  +   �� 5�  *   $��0��60   $��2��6@   $��Statistics Name��6M   6i   
  5�  *   -   5        $��txtStatName��5�  B  $��25��6@   $!3 StatisticsNameWaterMarkText��6L   $
��Auto��65  $��2��6U  $��1��6c  $��6,0,1,0q�6�  {�.  6 6n   �� *StatisticsNameWaterMarkText+$ PropertyChanged=�$ TwoWay=���6�   �� *StatisticsNameWaterMarkColor+$ PropertyChanged=�$ TwoWay=���6s  �� *StatisticsName+$ PropertyChanged=�$ TwoWay=���6�  �� *StatNameColSpan+.��6   5�  2     5�  6   $	 GotFocus��6E   .
 5�  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  7     5�  2   $	 	LostFocus��6A   .
 5�  6      6L   �� *TextBoxLostFocusCommand+ 6x   �� $ txtStatName��$	 Tag�6�   5�  3     5�  2   $	 TextChanged��6A   .
 5�  6      6L   �� *TextChangedCommand+6v   5�  3   .��5�  J   7 5�  .   �� 5�  2   .R�5�  6   �� $��Italic$�6Y   ��6E   �� 6n   5�  3   .��5�  W   5�  +   �� 5�  .   $��2��65   $��2��6B   $��3��6O   $��Search��6_   #��[ 6p   $��95��6�   $��25��6�   $��0,0,3,0q�6�   y�6�   �� *StatNameSearch+��6�   �� *StatisticsSearch++ 5�  0   �� 5�  2   
����������#ADAAAD$	��0.5q�6b   $��-5q�6x   ._�5�  6   �� -   5        $��SearchStats��5�  @   $��Search Statistics��6S   $��0��6l   $* 	10,5,10,0q�6y   $
��Bold"�6�   $��Calibri)�6�   6�   5�  3   5�  +   �� 5�  .   $��0��64   $��3��6D   $��Filter��6Q   6d   �� 5�  *   -   5        $��Ftype��5�  3   $��3��6@   $��23��6M   $	��242��6Y   $��3,0,0,0q�6e   {�. $��3��6�   $��1��6�   $��3��6�   ��6w   �� *FilterBorder+��6�   �� *FilterSource+$ TwoWay=�$ PropertyChanged=�5 64  �� *SelectedFilter+$ PropertyChanged=�$ TwoWay=� 5�  2     5�  2   $	 SelectionChanged��6A   .
 5�  6      6L   �� *FilterTypeChangedCommand+ 6y   �� $ Ftype��$ SelectedValue�6�   5�  3     5�  6   $	 GotFocus��6E   .
 5�  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  7   5�  /   �� 5�  .   $��0��64   $��4��6D   $��Format��6Q   6d   �� 5�  .   -   5        $��FormType��5�  7   $��4��6G   $��25��6T   $	��242��6`   $��3,0,0,0q�6m   {�. $��4��6�   $��1��6�   $��3��6�   ��6�   �� *FormatSource+$ PropertyChanged=�$ TwoWay=�5 6  �� *SelectedFormat+$ PropertyChanged=�$ TwoWay=� 5�  2     5�  6   $	 GotFocus��6E   .
 5�  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  7     5�  6   $	 SelectionChanged��6E   .
 5�  :      6P   �� *FormatTypeChanged+ 6v   �� $ FormType��$ SelectedItem�6�   5�  7   5�  /   �� 5�  .   $��0��64   $��5��6D   $��Tooltip value��6Q   6k   
  5�  *   -   5        $��TTValue��5�  @   $3 TooltipWaterMarkText��6O   $��5��6#  $��3,0,0,0q�61  $��25��6C  $	��242��6O  {�. $��5��6l  $��1��6z  $��3��6�  6 6j   �� *TooltipWaterMarkText+$ PropertyChanged=�$ TwoWay=���6�   �� *TooltipWaterMarkColor+$ PropertyChanged=�$ TwoWay=���6�  �� *TooltipValue+$ PropertyChanged=�$ TwoWay=�.��6�   5�  2     5�  6   $	 GotFocus��6E   .
 5�  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  7     5�  2   $	 	LostFocus��6A   .
 5�  6      6L   �� *TextBoxLostFocusCommand+ 6x   �� $ TTValue��$	 Tag�6�   5�  3     5�  2   $	 TextChanged��6A   .
 5�  6      6L   �� *TextChangedCommand+6v   5�  3   .��5�  J   7 5�  .   �� 5�  2   .R�5�  6   �� $��Italic$�6Y   ��6E   �� 6n   5�  3   .��5�  W   5�  +   �� 5�  .   $��0��64   $��6��6D   $��Statistics color��6Q   6n   #   Xceed.Wpf.Toolkit.ColorPicker  5�  .   $��25��6>   $��6��6J   $	��242��6W   $��1��6d   $��3,0,0,0q�6t   $��6��6�   $��3��6�   8   DisplayColorAndName8 . 9   SelectedColor9 6�   �� *StatisticsColor+ 5�  2     5�  6   $	 GotFocus��6E   .
 5�  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  7   5�  /   �� 5�  .   $��0��64   $��7��6D   $��Threshold level 1��6Q   6o   
  5�  *   -   5        $��
txtTvalue1��5�  @   $3 Threshold1WaterMarkText��6S   $��7��60  $��2��6=  $��1��6Q  $��7��6a  $��25��6o  $��3,0,0,0q�6{  $	��143��6�  6 6q   �� *Threshold1WaterMarkText+$ PropertyChanged=�$ TwoWay=���6�   �� *Threshold1WaterMarkColor+$ PropertyChanged=�$ TwoWay=���6�  �� *
Threshold1+$ PropertyChanged=�$ TwoWay=�.��6�   5�  2     5�  2   $	 	LostFocus��6A   .
 5�  6      6L   �� *TextBoxLostFocusCommand+ 6x   �� $ 
txtTvalue1��$	 Tag�6�   5�  3     5�  2   $	 GotFocus��6A   .
 5�  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  7     5�  6   $	 TextChanged��6E   .
 5�  6      6L   �� *TextChangedCommand+6v   5�  3   .��5�  J   7 5�  .   �� 5�  2   .R�5�  6   �� $��Italic$�6Y   ��6E   �� 6n   5�  3   .��5�  W   5�  /     5�  .   $��7��6>   $��8��6K   $��3��6X   $��25��6h   $��3,0,3,0q�6t   8 .  $��93��6�   9 6�   �� *ThresholdColor1+ 5�  2     5�  6   $	 GotFocus��6E   .
 5�  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5�  7   5  /   �� 5  .   $��0��64   $��8��6D   $��Threshold level 2��6Q   6o   
  5  *   -   5        $��
txtTValue2��5  @   $3 Threshold2WaterMarkText��6R   $��9��6/  $��1��6<  $��8��6L  $��2��6Y  $��25��6m  $	��143��6y  $��3,0,0,0q�6�  6 6p   �� *Threshold2WaterMarkText+$ PropertyChanged=�$ TwoWay=���6�   �� *Threshold2WaterMarkColor+$ PropertyChanged=�$ TwoWay=���6�  �� *
Threshold2+$ PropertyChanged=�$ TwoWay=�.��6�   5  2     5  2   $	 	LostFocus��6A   .
 5  6      6L   �� *TextBoxLostFocusCommand+ 6x   �� $ 
txtTValue2��$	 Tag�6�   5  3     5  2   $	 GotFocus��6A   .
 5	  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5
  7     5  6   $	 TextChanged��6E   .
 5  6      6L   �� *TextChangedCommand+6v   5  3   .��5  J   7 5  .   �� 5  2   .R�5  6   �� $��Italic$�6Y   ��6E   �� 6n   5  3   .��5  W   5  +     5  .   $��8��6>   $��10��6K   $��3��6Y   $��25��6i   $��3,0,3,0q�6u   8 .  $��93��6�   9 6�   �� *ThresholdColor2+ 5  2     5  6   $	 GotFocus��6E   .
 5  :      6P   �� *DataGridLostFocusCommand+ 6}   �� $ SearchStatnameDataGrid��$ SelectedValue�6�   5  7   5  /   � 5  .   -   5        $��SearchStatnameGrid��5  U   $��9��63   $��4��6@   $	��300��6o   $��3,0,3,0q�6|   .+�5  .     -   5        $��SearchStatnameDataGrid��5    $ Column=�67   $  Disabled=�6R   :   	RowHeight$: 25Q�6{    �#D6D7D6 �#D6D7D6$! Visible=�6�   $��0q�6�   $	��300��6�    .   .  #��[ 6�  
���������6_  �� *SearchStatnameCollection+��6�  �� *TitleBackground+# 5   6     5!  :   $	$ 160	 6Q   $) StatName��6]   % 5"  >   �� 5#  B   .R�5$  B   	� $��GridRadiobtn��6N   $��3,0,0,0q�6b   ; 	� 	GroupName$; 	StatNames��6s   ��6�   �� *SearchedStatname+$ TwoWay=�$ PropertyChanged=� 5%  J     5&  N   $	 Checked��6]   .
 5'  N      6d   �� *,*DataContext.DataGridRadiobtnCheckedCommand+' �� $( DataGridL� 6�   �� *SearchedStatnameTooltip+6  5(  O   + 5*  F   �� 5+  J   
����������#ADAAAD$	��0.5q�6z   $��-5q�6�   ._�5-  N   �� $��SectionNameAvail��6X   $��0��6�   $* 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6p   �� *SearchedStatnameTooltip+$ TwoWay=�$ PropertyChanged=�6  5/  K   51  C   52  C   54  ;     55  :   $	$ 188	 6Q   $) Stat Description��6]   % 56  >   �� 57  B   .R�59  F   �� $* 5,0,0,0q�6P   $��1,0,1,0q�6�   ��6b   �� *SearchedStatDescription+$ TwoWay=�$ PropertyChanged=�+ 5:     �� 5;     
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5=     �� $��DescriptionNameAvail��6$   $��0��6�   $* 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6@   �� *SearchedStatDescTooltip+$ TwoWay=�$ PropertyChanged=�6�   5?     5A  G   5B  C   5D  ;   5F  3   5G  /   5H  +   5I  '   5J  #   5K     5L     5M     5N     5O     5Q     5S     