   M S B A M L   `   `   `  ���� X  TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity I EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=nullE'clr-namespace:StatTickerFive.ConvertersStatTickerFive.Converters ?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers +   %StatTickerFive.Views.StatisticsWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jxcd:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4UxwpfJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  2conv'clr-namespace:StatTickerFive.Converters  0local$clr-namespace:StatTickerFive.Helpers  -   $��StatistcisWindow��  � Title$   StatisticsPropertiesWindow�� � SizeToContent$ Height=�$	��365�� � Topmost .  � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .  � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png������ *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ 	Activated��.  (System.Windows.Interactivity.TriggerBase	   Actions.	 6  0System.Windows.Interactivity.InvokeCommandAction  
   Command
 �� *ActivatedCommand+  $ Deactivated��.	   
 �� *DeactivateCommand+  $ MouseLeftButtonDown��.	   
 �� *	DragCmd+   CommandParameter ��  �� ElementName$ StatistcisWindow�� �� Path$ Title�  $ Loaded��.	   
 �� *WinLoadCommand+ � 	Resources  �� MergedDictionaries �� Source.  (StatTickerFive.Converters.MultiConverter   MultiValueConverters��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%   &	          .���� -   $��
MainBorder��$
��0.89q� �� CornerRadius$ 2��$	��364��$
��auto��
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $
��auto��$	��363�� � RowDefinitions �� $��25 ��� $��* �.+��� $��0��$ 2,2,0,0��$��25������ *TitleBackground+._�]� .+��� $��8,0,0,0q�$��Center=�$
��Left=�$
��Bold"�$��14(����� *Title+���� *TitleForeground+�� -   $��btnExit��$��Right=�$��20��$��20��  AdminCloseButton#��[ $��1q����� *PropertyClose+�� $��2q����#D6D7D6
�������$��1��$��2q�$
��auto��._�� $
��auto��$	��353�� �� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� ���� *DataGridRowHeight+�� $��10 ��� $��* � � ColumnDefinitions �� $	��125 ��� $��75 ��� $��75 ��� $��* �.+��� $��0��$��0��$��Section Name���� -   $��cboxSectionName��$��1��$��0��$��25��$��66��$��6,0,0,0q�{�. ���� *FilterBorder+���� *
Sections+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=� �� SelectedItem �� *SelectedSection+$ PropertyChanged=�$ TwoWay=�   $ SelectionChanged��.	   
 �� *SectionChanged+ �� $ cboxSectionName��$ SelectedItem��� -   $��txtSectionName�� �� 	UndoLimit$ 10��$��0��$��2��$��0��$	��150��$��25��$��2��$��-5,0,0,0q�
 � Tag �� *KeyToDifferentiateStyles+$ PropertyChanged=�$ TwoWay=����� *SectionName+$ PropertyChanged=�$ TwoWay=�.��   $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�.������  �� 
TargetType" �� �� Triggers P�  P� Property �@�Tag P� Value$ 1��. ���  �� Property � �� �� Value
 �  ����  � ��$ Italic$�P�  �@�Tag$ 0��. ���  � ��
 �������  � ��$ Italic$�P� , &StatTickerFive.Helpers.CustomValidator �@ IsWatermarked .  . ���  � ��
 �   ���  � ��$ Normal$�.��    	Behaviors    !   Text$! Enter a value here��.���� $��0��$��1��$��Display Name���� -   $��txtDisplayName��$ 10��$��1��$��25��$	��217��$��3,0,0,0q�$��1��$��1��$��3�� �� *KeyToDifferentiateStyles+$ PropertyChanged=�$ TwoWay=����� *DisplayName+$ PropertyChanged=�$ TwoWay=�.��   $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�.������ " �� P�  �@�Tag$ 1��. ���  � ��
 �  ����  � ��$ Italic$�P�  �@�Tag$ 0��. ���  � ��
 �������  � ��$ Italic$�P�  �@ IsWatermarked .  . ���  � ��
 �   ���  � ��$ Normal$�.��    $! Enter a value here��.���� $��0��$��2��$��Statistics Name���� -   $��txtStatName��$ 10��$��25��$
��Auto��$��2��$��1��$��6,0,1,0q�{�.   �� *KeyToDifferentiateStyles+$ PropertyChanged=�$ TwoWay=����� *StatisticsName+$ PropertyChanged=�$ TwoWay=����� *StatNameColSpan+.��   $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�.������ " �� P�  �@�Tag$ 1��. ���  � ��
 �  ����  � ��$ Italic$�P�  �@�Tag$ 0��. ���  � ��
 �������  � ��$ Italic$�P�  �@ IsWatermarked .  . ���  � ��
 �   ���  � ��$ Normal$�.��    $! Enter a value here��.���� $��2��$��2��$��3��$��Search��  NormalButton#��[ $��70��$��25��$��0,0,3,0q�y��� *StatNameSearch+���� *StatisticsSearch+�� $��0��$��3��$��Filter���� -   $��Ftype��$��3��$��23��$	��217��$��3,0,0,0q�{�. $��3��$��1��$��3������ *FilterBorder+���� *FilterSource+$ PropertyChanged=�$ TwoWay=� �� *SelectedFilter+$ PropertyChanged=�$ TwoWay=�   $ TextChanged��.	   
 �� *PropertyChanged+ �� $ Ftype��$
 Name�  $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue��� $��0��$��4��$��Format���� -	   $��FormType��$��4��$��25��$	��217��$��3,0,0,0q�{�. $��4��$��1��$��3������ *FormatSource+$ PropertyChanged=�$ TwoWay=� �� *SelectedFormat+$ PropertyChanged=�$ TwoWay=�   $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $ SelectionChanged��.	   
 �� *FormatTypeChanged+ �� $ FormType��$ SelectedItem��� $��0��$��5��$��Tooltip value���� -
   $��TTValue��$ 10��$��5��$��3,0,0,0q�$��25��$	��217��{�. $��5��$��1��$��3�� �� *KeyToDifferentiateStyles+$ PropertyChanged=�$ TwoWay=����� *TooltipValue+$ PropertyChanged=�$ TwoWay=�.��   $ TextChanged��.	   
 �� *PropertyChanged+ �� $ TTValue��$
 Name�  $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�.������ " �� P�  �@�Tag$ 1��. ���  � ��
 �  ����  � ��$ Italic$�P�  �@�Tag$ 0��. ���  � ��
 �������  � ��$ Italic$�P�  �@ IsWatermarked .  . ���  � ��
 �   ���  � ��$ Normal$�.��    $! Enter a value here��.���� $��0��$��6��$��Statistics color��#   Xceed.Wpf.Toolkit.ColorPicker  $��25��$��6��$	��217��$��1��$��3,0,0,0q�$��6��$��3��"   DisplayColorAndName" . #   SelectedColor# �� *StatisticsColor+   $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue��� $��0��$��7��$��Threshold level 1���� -   $��
txtTvalue1��$ 10��$��7��$��2��$��1��$��7��$��25��$��3,0,0,0q�$	��140�� �� *KeyToDifferentiateStyles+$ PropertyChanged=�$ TwoWay=����� *
Threshold1+$ PropertyChanged=�$ TwoWay=�.��   $ 	LostFocus��.	   
 �� *TextInputCommand+  $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�.������ " �� P�  �@�Tag$ 1��. ���  � ��
 �  ����  � ��$ Italic$�P�  �@�Tag$ 0��. ���  � ��
 �������  � ��$ Italic$�P�  �@ IsWatermarked .  . ���  � ��
 �   ���  � ��$ Normal$�.��    $! Enter a value here��.��  $��7��$��8��$��3��$��25��$��3,0,3,0q�" .  $��70��# �� *ThresholdColor1+   $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue��� $��0��$��8��$��Threshold level 2���� -   $��
txtTValue2��$ 10��$��9��$��1��$��8��$��2��$��25��$	��140��$��3,0,0,0q� �� *KeyToDifferentiateStyles+$ PropertyChanged=�$ TwoWay=����� *
Threshold2+$ PropertyChanged=�$ TwoWay=�.��   $ 	LostFocus��.	   
 �� *TextInputCommand+  $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�.������ " �� P�  �@�Tag$ 1��. ���  � ��
 �  ����  � ��$ Italic$�P�  �@�Tag$ 0��. ���  � ��
 �������  � ��$ Italic$�P�  �@ IsWatermarked .  . ���  � ��
 �   ���  � ��$ Normal$�.��    $! Enter a value here��.��  $��8��$��10��$��3��$��25��$��3,0,3,0q�" .  $��70��# �� *ThresholdColor2+   $ GotFocus��.	   
 �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�� -   $��SearchStatnameGrid��$��9��$��4��$��3,0,3,0q����� *DataGridRowHeight+$ TwoWay=�$ PropertyChanged=�.+�&   System.Windows.Controls.DataGrid  -   $��SearchStatnameDataGrid��$   HeadersVisibility$$ Column=�$%   HorizontalScrollBarVisibility$% Disabled=�&   	RowHeight$& 25Q�'   HorizontalGridLinesBrush' �#D6D7D6(   VerticalGridLinesBrush( �#D6D7D6")   VerticalScrollBarVisibility$) Visible=�$��0q�*   CanUserAddRows* .  +   AutoGenerateColumns+ .    
CustomGrid#��[ 
����������� $ SearchStatnameGrid��$ Height�$ TwoWay=�$ PropertyChanged=����� *SearchStatnameCollection+���� *TitleBackground+,   Columns, 4	  .System.Windows.Controls.DataGridTemplateColumn	  ,
  &System.Windows.Controls.DataGridColumn- 
  Width5  /System.Windows.Controls.DataGridLengthConverter$	- 128 . 
  Header$. Statistics Name��/ 	  CellTemplate/ �� .R�	� $��GridRadiobtn��$��3,0,0,0q�0 	� 	GroupName$0 	StatNames��1 � ToolTip1 �� *SearchedStatnameTooltip+$ TwoWay=�$ PropertyChanged=����� *SearchedStatname+$ TwoWay=�$ PropertyChanged=�   $ Checked��.	   
 �� *,*DataContext.DataGridRadiobtnCheckedCommand+2 �� RelativeSource2 �� 3 �� AncestorType$3 DataGridL� �� $ GridRadiobtn��$ ToolTip�	  $	- 199 $. Statistics Description��/ �� .R��� 4 �� Padding$4 5,0,0,0q�$��1,0,1,0q�1 �� *SearchedStatDescription+$ TwoWay=�$ PropertyChanged=����� *SearchedStatDescription+$ TwoWay=�$ PropertyChanged=�� $��11��$��3��$��0,0,7,6q� �� $	��175 ��� $��* �.+��� $��0��$��11��$��0,0,5,5q�$
��Save��#��[ $��Right=�$��75��$��23��{��� *IsSavebtnEnable+$ TwoWay=�$ PropertyChanged=����� *PropertySave+��`� 5 `� 	Converter#5 [  .1��� $ txtSectionName��$
 Text��� $ cboxSectionName��$ SelectedItem��� $ txtStatName��$
 Text�   $ 
MouseEnter��.	   
 �� *IsSavebtnEnabledCommand+�� $��1��$��12��$��5,0,0,5q�$��Cancel��#��[ $
��Left=�$��75��$��23������ *PropertyClose+