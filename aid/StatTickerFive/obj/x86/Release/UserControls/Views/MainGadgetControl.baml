   M S B A M L   `   `   `  ���� c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity   StatTickerFive*   $StatTickerFive.UserControls.Control1   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     -     >�Name$  Usercontrol��.   (System.Windows.Interactivity.Interaction   Triggers /   )System.Windows.Interactivity.EventTrigger     	EventName$ MouseDoubleClick��.   (System.Windows.Interactivity.TriggerBase   Actions. 6   0System.Windows.Interactivity.InvokeCommandAction     Command �� *WinDoubleClicked+ � 	Resources  �� MergedDictionaries �� Source��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%    .���� -   $��
MainBorder������ *
ThemeColor+���� *GadgetHeight+���� *GadgetWidth+y��� *MainCtrlVisibility+._��  � RowDefinitions �� $��25 ��� ���� *GridRowHeight+.+�� -   $��	GridTitle��$��0��$��23��$��2,0,2,0q�	 � ColumnDefinitions	 �� $��* ��� $��70 �.+��� -   $��StatTypeBox��$��0��$��Segoe UI)�$
��Auto��$��18��$��Center=�$
��Left=�
����������� *
StatType+
 � ToolTip
 �� *StatTypeTooltip+� $��1������ *TraverseButtonMargin+	 �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��1q�
���   �   NormalImageButton#��[  $��16��$��16��$��Center=�$��Center=�$��Right=����� *btnPrevious+.���� $��13��$��13������ *PreviousImg+
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��Previous Statistics��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��btnPlay��$��1��$��1q�
������ #��[  $��16��$��16��$��Center=�$��Center=�$��Right=����� *
btnPause+
 � Tag �� *	PlayPause+����  �� ElementName$ btnPlay�� �� Path$	 Tag�.���� $��13��$��13������ *PlayPauseImg+
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *PlayPauseTooltip+�� $��2��$��1q�
������ #��[  $��16��$��16��$��Center=�$��Center=�$��Right=����� *	btnNext+.���� $��13��$��13������ *	NextImg+
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��Next Statistics��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��BtnMenu��$��3��
������ #��[  $��0q�$��16��$��Center=�$��Center=�$��Right=�$��16�� � ContextMenu �� *BtnContextMenu+���� *	MenuClick+.���� $��13��$��Center=�$��Center=�$��13������ *	MenuImg+
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $
��Menu��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)�� -   $��GridStatistics��$��1��$��2,0,2,0q����� *GridBackgroundColor+
 �� *StatNameToolTip+���� *GridStatisticsHeight+	 �� $��* ��� $
��Auto ��� $
��Auto � �� $
��Auto ��� $��* �.+��� -   $��lblMainStatName��$��0��$��13(�$
��Left=�$��Stretch=�$��25��$��Segoe UI)����� *
StatWeight+���� *MainStatNameColor+���� *
StatName+ �� *
TagValue+���� *StatisticsMargin+
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *StatNameToolTip+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� -	   $��lblMainStatObj��$��1��$��Normal"�$��10(�$
��Left=�$��Stretch=�$��17��$��Segoe UI)�y��� *TagStatvaleVisible+���� *MainStatNameColor+���� *MainStatObject+ �� *
TagValue+���� *GridColumnMargin+���� *GridColumnSpan+� $��1��$��0������ *MainStatValueMargin+���� *GridValueHeight+���� *GridValeAlign+���� *MainStatValueColSpan+.+��� -
   $��lblMainValue��$��Center=�$��18(�$��28��$��Segoe UI)�$��Right=����� *
StatWeight+���� *	StatValue+���� *StatValueColor+���� *StatValMargin+� $��2��$��0��$��19������ *GridTagButtonHeight+y��� *TagVisibility+���� *GridVerticalAlign+���� *MainTagButtonRowSpan+.+��� $��18��$��20��
������ #��[  $��Center=�$��Center=����� *TagButtonMargin+���� *
TagClick+���� $ lblMainStatName��$	 Tag�.���� $��17��$��19��$2��,/StatTickerFive;component/Images/tag-add.png��$��Center=�$��Center=�
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��Tag Statistics��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)�