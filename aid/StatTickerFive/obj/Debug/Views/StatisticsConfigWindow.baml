   M S B A M L   `   `   `  ����I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=null?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	   1    +StatTickerFive.Views.StatisticsConfigWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Ge9http://schemas.microsoft.com/expression/2010/interactions     5   	   5c-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4UxwtkJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  5   	   0local$clr-namespace:StatTickerFive.Helpers  5
   	   XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  5   	   -   5        $��
UserConfig��5   )     � Title$  UserLevelConfigurations��6	   $
��Auto��6;   $
��Auto��6I    � SizeToContent$ WidthAndHeight=�6V    � Topmost .   � WindowStyle$
 None=�6�    � WindowStartupLocation$ CenterScreen=�6�    � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��5   	   ��5   �   �� *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ 	Activated��6   .  (System.Windows.Interactivity.TriggerBase	   Actions.	 5      6  0System.Windows.Interactivity.InvokeCommandAction  
   Command
 6$   �� *ActivatedCommand+6K   5        5   
   $ Deactivated��6   .	 5        
 6$   �� *DeactivateCommand+6L   5        5   
   $ MouseLeftButtonDown��6   .	 5        
 6$   �� *	DragCmd+   CommandParameter 6@   ��  �� ElementName$ 
UserConfig�� �� Path$ Title�6�   5        5   
   $ Loaded��6   .	 5        
 6$   �� *WinLoadCommand+6I   5       � 	Resources 5       �� MergedDictionaries �� Source    booleanToVisibilityConverter,   &StatTickerFive.Helpers.LengthConverter  dataGridLengthConverter�� 5   
    5      �� 5       $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�5!      %:   &	        &	      �� 5#      6R     5$      6F   5%      .��5'      �� $
��0.89q�6    �� CornerRadius$ 2��6$   $
��Auto��6   $
��Auto��6�   
���������65   �� *BorderBrush+ J� BitmapEffect 6Y   �� *ShadowEffect+._�5(   
   � $
��Auto��6   $	��705��6    � ColumnDefinitions 5)      �� 5*      $	��525 �6#   �� 5+      ��6#   �� *	GridWidth+ � RowDefinitions 5-      �� 5.      $��27 �6    6.   �� 5/      $��* �6    6-   .+�51      �� $��0��6   $��27��6I   $��0��6U   $
��Auto��6�   ��6"   �� *TitleBackground+��6e   �� *	TitleSpan+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�._�52      �  53      �� 54      $
��Auto �6+   �� 55      $��* �6+   �� 56      $��25 �6+   .+�59      �� $��0��6    $��8,3,0,0q�60   $��Statistics Configuration��6A   $
��Left=�6b   $��Stretch=�6}   $
��Bold"�6�   $��14(�6�   ��6�   �� *TitleForeground+6�   �� 5:      $��1��6    $��0,3,5,0q�60   $��Right=�6A   $
��Bold"�6~   $��14(�6�   ��6]   �� *ApplicationName+��6�   �� *TitleForeground+�� 5;      -   5        $��btnExit��5;   -   $��2��6   $��Center=�6<   $��20��6Y   $��20��6e     AdminCloseButton#��[ 6p   $��2q�6�   ��6�   �� *ConfigCancelCmd+5=      5>      �� 5?      $��1��6   
�������$��1q�65   ���#D6D7D6$	��669��6_   $	��518��6l   ._�5@      �� -   5        $��StatisticsTabControl��5@   L   $��1��6   $��2��6*   $	��664��6?   $	��515��6h    �� TabStripPlacement$	 Top=�6t   .@�5A      �� -   5        $��StatisticsTabItem��5A   <   $��Level Configurations��6   $
��Left=�6U   $
��Auto��6p   .��5B      �� 
�������$��1q�64   $��2q�6H   ���#D6D7D6$	��625��6i   $	��510��6v   ._�5C      � $
��Auto��6#   $
��Auto��61    5D   "   �� 5E   &   $��40 �64   6B   �� 5F   &   $��* �64   6A    5H   "   �� 5I   &   $��29 �67   �� 5J   &   $��* �67   .+�5M   "   �� ���#D6D7D6
�������$��1q�6R   $��0��6f   $��0��6s   $��3��6�   $��36��6�   $	��503��6�   $��2q�6�   ._�5N   &   � $	��495��60   $��5q�6<   .+�5O   *   �  5P   .   �� 5Q   2   $��75 �6C   6P   �� 5R   2   $��* �6C   6O   .+�5T   .   �� $��Search Object��68   $��0��6M   $
��auto��6]   $��Center=�6j   $
��Left=�6�   $��0q�6�   6�   1  +Microsoft.Windows.Controls.WatermarkTextBox  5U   .   $	��300��6D   $��1��6P   $
��Left=�6`      	Watermark$ Agent (or) AgentGroup��6�   ��6{   �� *SearchValue+$ PropertyChanged=�$ TwoWay=�.��6�    5V   2     5W   6   $ TextChanged��6E   .	 5X   :     
 6P   �� *SearchObjectCommand+6z   5Y   7   .��5Z   J   5[   /   5_   +   5`   '   5a   #   �� 5c   "   ���#D6D7D6
�������$	��Top=�6R   $��1q�6j   $��1��6~   $��0��6�   $��3��6�   $	��577��6�   $	��503��6�   $��2,2,2,0q�6�   ._�5d   &   �  5e   *   �� 5f   .   $��* �6<   6I   �� 5g   .   $��30 �6<   6J   .+�5i   &   �� $	��0.5q�6U   $��0��6k   $��2q�6y   $	��495��6�   ��6-   �� *TitleBackground+y�6�   �� *GridVisible+._�5j   *   �  5k   .   �� 5l   2   $��25 �6@   6N   �� 5m   2   $��* �6@   6M   .+�5o   .   �� $��0��65   $��1,1,1,1q�6B   $��0,0,0,0q�6\   ���#D6D7D6
�������6�   �� 5p   .   $��0��64   $��1��6A   $��Available Objects��6Q   $��Center=�6m   $
��Bold"�6�   $��Center=�6�   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5r   .   $	��492��6;   $��1��6G   .��5s   2   � $	��492��6<   $
��Auto��6H   .+�5t   6   &   System.Windows.Controls.DataGrid  -   5        $��ObjectsGridView��5t   o      CanUserReorderColumns .  $��-2,0,0,0q�6]      HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6
�������   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$
 None=�6     AutoGenerateColumns .      CanUserAddRows  .    
CustomGrid#��[ 6a  $	��495��6�  $	��545��6�  $!   HorizontalScrollBarVisibility$! Disabled=�6�  ""   VerticalScrollBarVisibility$" Disabled=�6�  #   
IsReadOnly# .  ��6  �� *ObjectCollection+$ �� NotifyOnTargetUpdated$ . $ PropertyChanged=�$ TwoWay=�%   Columns% 5u   :   4  .System.Windows.Controls.DataGridTemplateColumn  5v   >   ,	  &System.Windows.Controls.DataGridColumn& 	  Width5
  /System.Windows.Controls.DataGridLengthConverter$& 25
 6U   '   CellTemplate' 5w   B   �� 5x   F   .R�5y   J   �� $��chkbox��6S   $��5q�6a   $��20��6l   ( d� 	IsChecked( 6w   �� *isGridChecked+$ TwoWay=�$ PropertyChanged=� 5z   N     5{   R   $ Checked��6a   .	 5|   V     
 6l   �� *#!DataContext.ObjectSelectedCommand+) �� RelativeSource) �� * �� AncestorType$* DataGridL� 6�   �� 6�   5}   S     5~   R   $ 	Unchecked��6a   .	 5   V     
 6l   �� *%#DataContext.ObjectDeselectedCommand+) �� $* DataGridL� 6�   �� 6�   5�   S   5�   K   5�   G   5�   ?     5�   >   $	& 450
 6U   + 	  Header$+ Available Objects��6a   ' 5�   B   �� 5�   F   .R�5�   J   �� $��
lblObjName��6T   , �� Padding$, 5,0,0,0q�6f   ��6x   �� *
ObjectName+$ TwoWay=�$ PropertyChanged=�- � ToolTip- 5�   O   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�      �� $��ObjectsAvail��6$   $��0��6�   $, 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��68   �� *
ObjectName+$ TwoWay=�$ PropertyChanged=�6�   5�      5�   K   5�   G   5�   ?   5�   7   5�   3   5�   /   5�   +   5�   '   � 5�   *   $��1��6/    5�   .   �� 5�   2   $	��235 �6C   6Q   �� 5�   2   $��* �6C   6O   .+�5�   .   �� $��0��65     NormalButton#��[ 6E   $
��Save��6l   $��23��6{   $��75��6�   $��Right=�6�   $��0,5,2,2q�6�   ��6�   �� *SaveStatistics+- 5�   .   �� 5�   2   
����������#ADAAAD$	��0.5q�6b   $��-5q�6x   ._�5�   6   �� -   5        $��SaveSettings��5�   @   $��Save Settings��6T   $��0��6i   $, 	10,5,10,0q�6v   $
��Bold"�6�   $��Calibri)�6�   6�   5�   3   5�   +   �� 5�   *   $��1��61   #��[ 6A   $
��Skip��6h   $��23��6w   $��75��6�   $
��Left=�6�   $��2,5,0,2q�6�   ��6�   �� *ConfigSkipCmd+- 5�   .   �� 5�   2   
����������#ADAAAD$	��0.5q�6b   $��-5q�6x   ._�5�   6   �� -   5        $��SkipSettings��5�   @   $��Skip Settings��6T   $��0��6i   $, 	10,5,10,0q�6v   $
��Bold"�6�   $��Calibri)�6�   6�   5�   3   5�   /   5�   +   5�   '   5�   #   5�      5�      5�      5�      5�      �� 5�      ���#D6D7D6$	��Top=�6+   
�������$��1q�6V   $��1��6j   $��1��6w   $	��675��6�   $	��495��6�   $��2,2,2,0q�6�   ._�5�      �� $	��0.5q�6A   $��2��6W   $��0��6d   $��2��6t   $��2q�6�   $	��488��6�   ��6   �� *TitleBackground+._�5�      �� $
��Left=�6"   $	��660��6=   $	��480��6J   $
 Left=�6V   $��3q�6�     ScrolltabStyle#��[ 6�   . �� SelectedIndex. 6o   �� *ObjectIndex+��6�   �� *	TabValues+$ PropertyChanged=�$ TwoWay=�6�   5�      5�      5�      5�      5�      