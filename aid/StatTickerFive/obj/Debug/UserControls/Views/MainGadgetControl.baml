   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5       StatTickerFive*   $StatTickerFive.UserControls.Control1   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5      -   5          >�Name$  Usercontrol��5      .   (System.Windows.Interactivity.Interaction   Triggers 5	      /   )System.Windows.Interactivity.EventTrigger  5
   
      	EventName$ MouseDoubleClick��6   .   (System.Windows.Interactivity.TriggerBase   Actions. 5      6   0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *WinDoubleClicked+6K   5       � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�6%   6g   %    5      .��5      �� -   5        $��
MainBorder��5   m   ��6   �� *
ThemeColor+��6/   �� *GadgetHeight+��6O   �� *GadgetWidth+y�6   �� *MainCtrlVisibility+._�5   
   �  � RowDefinitions 5      �� 5      $��25 �6    �� 5      ��6    �� *GridRowHeight+.+�5      � -   5        $��	GridTitle��5   =   $��0��6   $��23��6    $��2,0,2,0q�6,   	 � ColumnDefinitions	 5      �� 5       $��* �6'   63   �� 5!      $��70 �6'   64   .+�5$      �� -   5        $��StatTypeBox��5$   ,   $��0��6   $��Segoe UI)�6?   $
��Auto��6V   $��18��6~   $��Center=�6�   $
��Left=�6�   
���������6d   �� *
StatType+
 � ToolTip
 6�   �� *StatTypeTooltip+ 5%       �� 
TargetType �� BasedOn �� Property �� Value �� 
TargetType��%  '_�       7_��� 5&      " _�6    8   .�5'      ��  � ��$ SemiBold"�6;   �� 5(       � �� 5)   "   �� 5*   &   " _�66   .R�5+   *   �� 
����������#ADAAAD$��1q�6Z   ._�5,   .   �� #��z��6@   $��5q�6d   5-   +   5.   '   50      51      52      53      � 55      $��1��6   ��6'   �� *TraverseButtonMargin+	 56      �� 57      $
��Auto �6+   �� 58      $
��Auto �6+   �� 59      $
��Auto �6+   �� 5:      $
��Auto �6+   .+�5=      �� $��0��6   $��1q�6.   $
 Previous Statistics��69   
���   �   NormalImageButton#��[  6j   $��16��6�   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6�   ��6  �� *btnPrevious+.��5>      �� $��13��6!   $��13��6-   ��68   �� *PreviousImg+6Y    5F      ��%  '_�       7_��� 5G      " _�6$   8   .�5H   "   ��  � ��$ SemiBold"�6?   �� 5I   "    � �� 5J   &   �� 5K   *   " _�6:   .R�5L   .   �� 
����������#ADAAAD$��1q�6^   ._�5M   2   �� #��z��6D   $��5q�6h   5N   /   5O   +   5Q   #   5R      5S      5T      �� 5U      -   5        $��btnPlay��5U   ]   $��1��6   $��1q�6R   
������ #��[  6�   $��16��6�   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6  
 6-   �� *PlayPauseTooltip+��6!  �� *
btnPause+
 � Tag 6>  �� *	PlayPause+��6X  ��  �� ElementName$ btnPlay�� �� Path$	 Tag�.��5V      �� $��13��6!   $��13��6-   ��68   �� *PlayPauseImg+6Z    5^      ��%  '_�       7_��� 5_      " _�6$   8   .�5`   "   ��  � ��$ SemiBold"�6?   �� 5a   "    � �� 5b   &   �� 5c   *   " _�6:   .R�5d   .   �� 
����������#ADAAAD$��1q�6^   ._�5e   2   �� #��z��6D   $��5q�6h   5f   /   5g   +   5i   #   5j      5k      5l      �� 5m      $��2��6   $
 Next Statistics��6-   $��1q�6G   
������ #��[  6k   $��16��6�   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6�   ��6  �� *	btnNext+.��5n      �� $��13��6    $��13��6,   ��67   �� *	NextImg+6T    5v      ��%  '_�       7_��� 5w      " _�6$   8   .�5x   "   ��  � ��$ SemiBold"�6?   �� 5y   "    � �� 5z   &   �� 5{   *   " _�6:   .R�5|   .   �� 
����������#ADAAAD$��1q�6^   ._�5}   2   �� #��z��6D   $��5q�6h   5~   /   5   +   5�   #   5�      5�      5�      �� 5�      -   5        $��BtnMenu��5�   <   $��3��6   $

 Menu��6-   
������ #��[  6d   $��0q�6�   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6�   $��16��6D   � ContextMenu 6�   �� *BtnContextMenu+��6&  �� *	MenuClick+.��5�      �� $��13��6    $��Center=�6,   $��Center=�6G   $��13��6d   ��6o   �� *	MenuImg+6�    5�      ��%  '_�       7_��� 5�      " _�6$   8   .�5�   "   ��  � ��$ SemiBold"�6?   �� 5�   "    � �� 5�   &   �� 5�   *   " _�6:   .R�5�   .   �� 
����������#ADAAAD$��1q�6^   ._�5�   2   �� #��z��6D   $��5q�6h   5�   /   5�   +   5�   #   5�      5�      5�      5�      5�      � 5�      -   5        $��GridStatistics��5�       $��1��6   $��2,0,2,0q�6b   ��66   �� *GridBackgroundColor+��6s   �� *GridStatisticsHeight+	 5�      �� 5�      $��* �6'   �� 5�      $
��Auto �6'   �� 5�      $
��Auto �6'    5�      �� 5�      $
��Auto �6$   64   �� 5�      $��* �6$   61   .+�5�      �� -   5        $��lblMainStatName��5�      $��0��63   $��13(�6�   $
��Left=�6
  $��Stretch=�6%  $��25��6A  $��Segoe UI)�6f  
 6@   �� *StatNameToolTip+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=���6�   �� *
StatWeight+��6�   �� *MainStatNameColor+��6�   �� *
StatName+ 6M  �� *
TagValue+��6|  �� *StatisticsMargin+ 5�      ��%  '_�       7_��� 5�      " _�6    8   .�5�      ��  � ��$ SemiBold"�6;   �� 5�       � �� 5�   "   �� 5�   &   " _�66   .R�5�   *   �� 
����������#ADAAAD$��1q�6Z   ._�5�   .   �� #��z��6@   $��5q�6d   5�   +   5�   '   5�      5�      5�      5�      �� 5�      -	   5        $��lblMainStatObj��5�      $
 Statistics Object��62   $��1��6x   $��Normal"�6�   $��10(�6�   $
��Left=�6�   $��Stretch=�6  $��17��6+  $��Segoe UI)�6P  y�6N   �� *TagStatvaleVisible+��6�   �� *MainStatNameColor+��6�   �� *MainStatObject+ 67  �� *
TagValue+��6g  �� *GridColumnMargin+��6�  �� *GridColumnSpan+ 5�      ��%  '_�       7_��� 5�      " _�6    8   .�5�      ��  � ��$ SemiBold"�6;   �� 5�       � �� 5�   "   �� 5�   &   " _�66   .R�5�   *   �� 
����������#ADAAAD$��1q�6Z   ._�5�   .   �� #��z��6@   $��5q�6d   5�   +   5�   '   5�      5�      5�      5�      � 5�      $��1��6   $��0��6'   ��64   �� *MainStatValueMargin+��6[   �� *GridValueHeight+��6~   �� *GridValeAlign+��6�   �� *MainStatValueColSpan+.+�5�      �� -
   5        $��lblMainValue��5�       $
 Statistics Value��64   $��Center=�6�   $��18(�6�   $��28��6�   $��Segoe UI)�6�   $��Right=�6�   ��6O   �� *
StatWeight+��6q   �� *	StatValue+��6�   �� *StatValueColor+��6  �� *StatValMargin+ 5�      ��%  '_�       7_��� 5�      " _�6    8   .�5�      ��  � ��$ SemiBold"�6;   �� 5�       � �� 5�   "   �� 5�   &   " _�66   .R�5�   *   �� 
����������#ADAAAD$��1q�6Z   ._�5�   .   �� #��z��6@   $��5q�6d   5�   +   5�   '   5�      5�      5�      5�      5�      � 5�      $��2��6   $��0��6'   $��19��64   ��6?   �� *GridTagButtonHeight+y�6f   �� *TagVisibility+��6�   �� *GridVerticalAlign+��6�   �� *MainTagButtonRowSpan+.+�5�      �� $��18��6   $��20��6(   $
 Tag Statistics��64   
������ #��[  6�   $��Center=�6  $��Center=�67  ��6M   �� *TagButtonMargin+��6�   �� *
TagClick+��6�   �� $ lblMainStatName��$	 Tag�.��5�      �� $��17��6!   $��19��6,   $2��,/StatTickerFive;component/Images/tag-add.png��68   $��Center=�6n   $��Center=�6�   6�    5     ��%  '_�       7_��� 5     " _�6$   8   .�5	  "   ��  � ��$ SemiBold"�6?   �� 5
  "    � �� 5  &   �� 5  *   " _�6:   .R�5  .   �� 
����������#ADAAAD$��1q�6^   ._�5  2   �� #��z��6D   $��5q�6h   5  /   5  +   5  #   5     5     5     5     5     5     5     5     