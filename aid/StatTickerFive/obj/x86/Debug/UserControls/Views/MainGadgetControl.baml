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
 6�   �� *StatTypeTooltip+6�   � 5&      $��1��6   ��6'   �� *TraverseButtonMargin+	 5'      �� 5(      $
��Auto �6+   �� 5)      $
��Auto �6+   �� 5*      $
��Auto �6+   �� 5+      $
��Auto �6+   .+�5.      �� $��0��6   $��1q�6.   
���   �   NormalImageButton#��[  6L   $��16��6w   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6�   ��6�   �� *btnPrevious+.��5/      �� $��13��6!   $��13��6-   ��68   �� *PreviousImg+6Y   
 50      �� 51      
����������#ADAAAD$	��0.5q�6N   $��-5q�6d   ._�53   "   �� $��Previous Statistics��6,   $��0��6G    �� Padding$ 	10,5,10,0q�6T   $
��Bold"�6h   $��Calibri)�6z   6�   55      57      �� 58      -   5        $��btnPlay��58   8   $��1��6   $��1q�6-   
������ #��[  6a   $��16��6�   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6�   ��6�   �� *
btnPause+
 � Tag 6  �� *	PlayPause+��63  ��  �� ElementName$ btnPlay�� �� Path$	 Tag�.��59      �� $��13��6!   $��13��6-   ��68   �� *PlayPauseImg+6Z   
 5:      �� 5;      
����������#ADAAAD$	��0.5q�6N   $��-5q�6d   ._�5=   "   �� $��0��6N   $ 	10,5,10,0q�6[   $
��Bold"�6o   $��Calibri)�6�   ��6,   �� *PlayPauseTooltip+6�   5?      5A      �� 5B      $��2��6   $��1q�6-   
������ #��[  6Q   $��16��6|   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6�   ��6�   �� *	btnNext+.��5C      �� $��13��6    $��13��6,   ��67   �� *	NextImg+6T   
 5D      �� 5E      
����������#ADAAAD$	��0.5q�6N   $��-5q�6d   ._�5G   "   �� $��Next Statistics��6,   $��0��6C   $ 	10,5,10,0q�6P   $
��Bold"�6d   $��Calibri)�6v   6�   5I      5K      �� 5M      -   5        $��BtnMenu��5M   -   $��3��6   
������ #��[  6U   $��0q�6�   $��16��6�   $��Center=�6�   $��Center=�6�   $��Right=�6�   $��16��65   � ContextMenu 6�   �� *BtnContextMenu+��6  �� *	MenuClick+.��5N      �� $��13��6    $��Center=�6,   $��Center=�6G   $��13��6d   ��6o   �� *	MenuImg+6�   
 5O      �� 5P      
����������#ADAAAD$	��0.5q�6N   $��-5q�6d   ._�5R   "   �� $
��Menu��6,   $��0��68   $ 	10,5,10,0q�6E   $
��Bold"�6Y   $��Calibri)�6k   6�   5T      5W      5X      5Z      � 5[      -   5        $��GridStatistics��5[       $��1��6   $��2,0,2,0q�6�   ��66   �� *GridBackgroundColor+
 6a   �� *StatNameToolTip+��6�   �� *GridStatisticsHeight+	 5]      �� 5^      $��* �6'   �� 5_      $
��Auto �6'   �� 5`      $
��Auto �6'    5b      �� 5c      $
��Auto �6$   64   �� 5d      $��* �6$   61   .+�5l      �� -   5        $��lblMainStatName��5l      $��0��63   $��13(�6�   $
��Left=�6�   $��Stretch=�6�   $��25��6�   $��Segoe UI)�6  ��6@   �� *
StatWeight+��6b   �� *MainStatNameColor+��6�   �� *
StatName+ 6�   �� *
TagValue+��6&  �� *StatisticsMargin+
 5m      �� 5n      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5p      �� $��0��6w   $ 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6$   �� *StatNameToolTip+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   5r      5t      �� 5u      -	   5        $��lblMainStatObj��5u      $��1��6\   $��Normal"�6i   $��10(�6�   $
��Left=�6�   $��Stretch=�6�   $��17��6  $��Segoe UI)�64  y�62   �� *TagStatvaleVisible+��6~   �� *MainStatNameColor+��6�   �� *MainStatObject+ 6  �� *
TagValue+��6K  �� *GridColumnMargin+��6o  �� *GridColumnSpan+5w      � 5z      $��1��6   $��0��6'   ��64   �� *MainStatValueMargin+��6[   �� *GridValueHeight+��6~   �� *GridValeAlign+��6�   �� *MainStatValueColSpan+.+�5{      �� -
   5        $��lblMainValue��5{       $��Center=�6r   $��18(�6�   $��28��6�   $��Segoe UI)�6�   $��Right=�6�   ��64   �� *
StatWeight+��6V   �� *	StatValue+��6�   �� *StatValueColor+��6�   �� *StatValMargin+5}      5~      � 5�      $��2��6   $��0��6'   $��19��64   ��6?   �� *GridTagButtonHeight+y�6f   �� *TagVisibility+��6�   �� *GridVerticalAlign+��6�   �� *MainTagButtonRowSpan+.+�5�      �� $��18��6   $��20��6(   
������ #��[  6p   $��Center=�6�   $��Center=�6  ��64   �� *TagButtonMargin+��6�   �� *
TagClick+��6�   �� $ lblMainStatName��$	 Tag�.��5�      �� $��17��6!   $��19��6,   $2��,/StatTickerFive;component/Images/tag-add.png��68   $��Center=�6n   $��Center=�6�   6�   
 5�      �� 5�      
����������#ADAAAD$	��0.5q�6N   $��-5q�6d   ._�5�   "   �� $��Tag Statistics��6,   $��0��6B   $ 	10,5,10,0q�6O   $
��Bold"�6c   $��Calibri)�6u   6�   5�      5�      5�      5�      5�      5�      5�      