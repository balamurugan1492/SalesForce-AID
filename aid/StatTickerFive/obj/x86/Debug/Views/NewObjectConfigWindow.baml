   M S B A M L   `   `   `  ����I  EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=nullE'clr-namespace:StatTickerFive.ConvertersStatTickerFive.Converters  5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5
   	   0    *StatTickerFive.Views.NewObjectConfigWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Ge9http://schemas.microsoft.com/expression/2010/interactions     5   	   5c-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   3local'clr-namespace:StatTickerFive.Converters  5   	   XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  5
   	   -   5        $��NewObjectConfiguration��5   !     � Title$  NewObjectConfig��6	   $
��Auto��6?   $
��Auto��6M    � SizeToContent$ WidthAndHeight=�6Z    � Topmost .   � WindowStyle$
 None=�6�    � WindowStartupLocation$ CenterScreen=�6�    � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��5   	   ��5   �   �� *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ 	Activated��6   .  (System.Windows.Interactivity.TriggerBase	   Actions.	 5      6  0System.Windows.Interactivity.InvokeCommandAction  
   Command
 6$   �� *ActivatedCommand+6K   5        5   
   $ Deactivated��6   .	 5        
 6$   �� *DeactivateCommand+6L   5        5   
   $ MouseLeftButtonDown��6   .	 5        
 6$   �� *	DragCmd+   CommandParameter 6@   ��  �� ElementName$ NewObjectConfiguration�� �� Path$ Title�6�   5        5   
   $ Loaded��6   .	 5        
 6$   �� *WinLoadCommand+6I   5       � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5       $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�5!      %    5#      .��5&      �� $
��0.89q�6    �� CornerRadius$ 2��6$   $	��480��6   $	��200��6�   
���������65   �� *BorderBrush+ J� BitmapEffect 6Y   �� *ShadowEffect+._�5'   
   � $	��479��6   $	��198��6    � RowDefinitions 5(      �� 5)      $��27 �6    6.   �� 5*      $��27 �6    6.   �� 5+      $��* �6    6-   .+�5-      �� $ 2,2,0,0��6   $��27��6S   $��0��6_   ��6,   �� *TitleBackground+._�5.      �  � ColumnDefinitions 5/      �� 50      $
��Auto �6+   �� 51      $��* �6+   �� 52      $��25 �6+   .+�54      �� $��0��6    $��8,3,0,0q�60   $��Select Object��6A   $
��Left=�6W   $��Stretch=�6r   $
��Bold"�6�   $��14(�6�   ��6�   �� *TitleForeground+�� 55      -   5        $��btnExit��55   -   $��2��6   $��Center=�6=   $��20��6Z   $��20��6f      AdminCloseButton#��[  6q   $��2q�6�   ��6�   �� *ConfigCancelCmd+��6�   �� $ NewObjectConfiguration��$ Title�56      57      �� 59      $��1��6   $��5,0,0,0q�6"   $��20��63   $
��Left=�6>   $��20��6Y   
������   ImageButton#��[ 6�    � ToolTip$
 Save��6
  ��6e   �� *	SaveCmd+��6�   �� $ NewObjectConfiguration��$ Title�.��5:      �� $/��)/StatTickerFive;component/Images/save.png��6   $��19��6K   $��20��6W   6e   5;      �� 5=      $��2��6=   $	��Top=�6J   
�������$��1q�6u   $��2q�6�   $	��140��6�   ��6   �� *TitleBackground+._�5>      �� $	��Top=�6A   
�������$��1q�6l   $��2q�6�   $	��135��6�   ��6   �� *TitleBackground+._�5?      �  5@      �� 5A      $��25 �6(   66   �� 5B      $��* �6(   65    5D      �� 5E      $	��225 �6+   69   �� 5F      $��* �6+   67   .+�5H      �� $��0��6   $��0��6*   $��1,1,1,1q�6:   ���#D6D7D6
�������6   �� 5I      $��0��6   $��0��6)   $��Object Name��69   $��Center=�6O   $
��Bold"�6j   $��Center=�6|   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5J      $��0��6   $��1��6*   $��1,1,1,1q�6:   ���#D6D7D6
�������6   �� 5K      $��0��6   $��1��6)   $��Switch Name��69   $��Center=�6O   $
��Bold"�6j   $��Center=�6|   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5L      $��1��6#   $��0��60   $��2��6@   .��5M      � .+�5N   "   � -   5        $��ProxyElement��5N   3   $y�	Collapsed=�6I   &   System.Windows.Controls.DataGrid  5O   "   -   5        $��ObjectsGridView��5O   J      CanUserReorderColumns .     HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6
�������   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$
 None=�6�      AutoGenerateColumns .     CanUserAddRows .    
CustomGrid#��[ 6<  $	��465��6`  $   HorizontalScrollBarVisibility$ Disabled=�6l  "   VerticalScrollBarVisibility$ Hidden=�6�  ��6�  �� *SelectedObject+  �� Mode$  TwoWay=�! �� UpdateSourceTrigger$! PropertyChanged=�"   Columns" 5P   &   4  .System.Windows.Controls.DataGridTemplateColumn  5Q   *   ,  &System.Windows.Controls.DataGridColumn#   Width5  /System.Windows.Controls.DataGridLengthConverter$# 25 6A   $   CellTemplate$ 5R   .   �� 5S   2   .R�5T   6   �� $��chkbox��6?   $��5q�5U   �   $��20��6�   ��5T   N   �� *DataContext.NewObjectSelected+% �� RelativeSource% �� & �� AncestorType$& DataGridL���5U   /   �� ' d� 	IsChecked' 6L   �� *IsObjectChecked+$  TwoWay=�$! PropertyChanged=�6�   5V   3   5X   +     5[   *   (   Header$( ACDQueue��6A   $	# 200 6S   $ 5^   .   �� 5`   2   .R�5a   6   �� $��lblStatName��6R   ) �� Padding$) 5,0,0,0q�6@   ��6e   �� *
ObjectName+$  TwoWay=�$! PropertyChanged=� 6�   �� *
ObjectName+$  TwoWay=�$! PropertyChanged=�6  5b   3   5g   +     5i   *   $( Switch Name��6A   $	# 223 6V   $ 5k   .   �� 5l   2   .R�5m   6   �� $��lblSwitchName��6R   $) 5,0,0,0q�6@   ��6g   �� *ObjectSwitchName+$  TwoWay=�$! PropertyChanged=� 6�   �� *ObjectSwitchName+$  TwoWay=�$! PropertyChanged=�5n   7   5o   3   5q   +   5u   #   5v      5w      5x      5z      5{      5|      5}      5      