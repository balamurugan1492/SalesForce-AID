   M S B A M L   `   `   `  ���� X  TXceed.Wpf.Toolkit, Version=2.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  I EStatTickerFive, Version=5.0.3.2, Culture=neutral, PublicKeyToken=nullE'clr-namespace:StatTickerFive.ConvertersStatTickerFive.Converters ?$clr-namespace:StatTickerFive.HelpersStatTickerFive.Helpers c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity ,   &StatTickerFive.Views.AdminConfigWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Ge9http://schemas.microsoft.com/expression/2010/interactions     5c-http://schemas.microsoft.com/wpf/2008/toolkit  Dxcd:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit  2conv'clr-namespace:StatTickerFive.Converters  Z VWPFToolkit.Extended, Version=1.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4UxwtkJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended  0local$clr-namespace:StatTickerFive.Helpers  XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  -   $��AdminConfig��  � Title$  AdminConfigurations��$
��Auto�� � 
ResizeMode$ NoResize=�$
��Auto�� � SizeToContent$ WidthAndHeight=� � Topmost .   � WindowStyle$
 None=� � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .   � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png������ *BackgroundColor+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger  	   	EventName$	 	Activated��.  (System.Windows.Interactivity.TriggerBase
   Actions.
 6  0System.Windows.Interactivity.InvokeCommandAction     Command �� *ActivatedCommand+  $	 Deactivated��.
    �� *DeactivateCommand+  $	 MouseLeftButtonDown��.
    �� *	DragCmd+   CommandParameter ��  �� ElementName$ AdminConfig�� �� Path$ Title�  $	 Loaded��.
    �� *WinLoadCommand+ � 	Resources  �� MergedDictionaries �� Source.  (StatTickerFive.Converters.MultiConverter   MultiValueConverters��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%   &	          .���� $
��0.89q� �� CornerRadius$ 2��$
��Auto��$	��705��
����������� *BorderBrush+ J� BitmapEffect �� *ShadowEffect+._�� $
��Auto��$	��705�� � RowDefinitions �� $��28 ��� $��* � � ColumnDefinitions �� $	��525 ��� ���� *	GridWidth+.+��� $��0��$��30��$��0��$
��Auto������ *TitleBackground+���� *	TitleSpan+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�._��  �� $
��Auto ��� $��* ��� $��25 �.+��� $��0��$��8,3,0,0q�$��Admin Configuration��$
��Left=�$��Stretch=�$
��Bold"�$��14(����� *TitleForeground+�� $��1��$��0,3,5,0q�$��Right=�$
��Bold"�$��14(����� *ApplicationName+���� *TitleForeground+�� -   $��btnExit��$��2��$��Center=�$��20��$��20��  AdminCloseButton#��[ $��2q����� *ConfigSkipCmd+�� $��1��
�������$��1q����#D6D7D6$	��669��$	��518��._��� -   $��AdminTabControl��$��3q�$	��661��$	��510�� �� TabStripPlacement$	 Top=�.@��� -   $��ApplicationTabItem��$��Application��$
��Left=�.��� $	��628��$	��504�� �� $	��600 ��� $��30 � �� $	��255 ��� $��* �.+��� $	��0.5q�$��0��$��0��$��3��$��0q����� *TitleBackground+._��  �� $��25 ��� $��* � �� $��25 ��� $	��190 ��� $��70 ��� $	��195 ��� $��19 �.+��� $��0��$��0��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
��������� $��1��$��0��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
��������� $��2��$��0��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
��������� $��3��$��0��$��1,1,1,1q�$��0,0,0,0q����#D6D7D6
��������� $��0��$��1��$��Section/Statistics Name��$��Center=�$
��Bold"�$��Center=�$��12(�$��Calibri)�
���   ��� $��0��$��2��$��
Properties��$��Center=�$��Center=�$
��Bold"�$��12(�$��Calibri)�
���   ��� $��0��$��3��$��Description��$��Center=�$��Center=�$
��Bold"�$��12(�$��Calibri)�
���   ��� $��1��$��0��$��5��.��� $	��508��.+�&   System.Windows.Controls.DataGrid  -   $"��ConfiguredStatisticsGridView��   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   CanUserResizeColumns .     CanUserResizeRows .     HeadersVisibility$
 None=�   AutoGenerateColumns .     CanUserAddRows .    
CustomGrid#��[ $	��490��$    HorizontalScrollBarVisibility$  Disabled=�"!   VerticalScrollBarVisibility$! Disabled=�"   RowBackground" �� *ExistingColor+$ PropertyChanged=�$ TwoWay=����� *ConfiguredStatistics+#   Columns# 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn$   Width5	  /System.Windows.Controls.DataGridLengthConverter$$ 25	 %   CellTemplate% �� .R��� $��chkbox��$��5q�$��20��& d� 	IsChecked& �� *isGridChecked+$ TwoWay=�$ PropertyChanged=����� *$"DataContext.ConfiguredStatSelected+' �� RelativeSource' �� ( �� AncestorType$( DataGridL�����   $	$ 190	 )   Header$) Section/Statistics Name��% �� .R��� $��tblConfiguredStatName��* �� Padding$* 5,0,0,0q����� *SectionName+$ TwoWay=�$ PropertyChanged=�+ � ToolTip+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameExist��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)����� *SectionTooltip+$ TwoWay=�$ PropertyChanged=�  $$ 70	 $) 
Properties��% �� .R��� $��btnEdit��  NormalImageButton#��[ $��60��$+ Edit Statistics������ *$"DataContext.EditExistingStatistics+' �� $( DataGridL����� .���� $
��Left=�, �� Orientation$, 
Horizontal=�.+��� $3��-/StatTickerFive;component/Images/editIcon.png��$
��Left=�$��17��$��20���� $��Calibri)�$��14(�
���   ����� *
EditName++ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��	EditAvail��$ ��Edit Statistics Properties��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)�- �� Triggers- �� . �� Value$
. true��/ �� Binding/ �� *isGridChecked+.a��� 0 �� 
TargetName$0 btnEdit��1 �� Property1 � {�2 �� Value2 . �� $. false��/ �� *isGridChecked+.a��� $0 btnEdit��1 � {�2 .    $	$ 195	 $) Description��% �� .R��� $��tblDisplayName��$* 5,0,0,0q����� *DisplayName+$ TwoWay=�$ PropertyChanged=�+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��DisplayNameAvail��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)����� *DisplayTooltip+$ TwoWay=�$ PropertyChanged=�  -   $��StatisticsServerGridView�� �#D6D7D6 �#D6D7D6 .   .  $
 None=� .   .  #��[ $	��507��$  Disabled=�$! Disabled=�" �� *ServerColor+$ PropertyChanged=�$ TwoWay=����� *ServerStatistics+#   $$ 25	 % �� .R��� $��chkbox��
3 � Tag$
3 null��$��5q�$��20��& �� *isGridChecked+$ TwoWay=�$ PropertyChanged=����� *&$DataContext.ActivatedCheckboxCommand+' �� $( DataGridL�����   $	$ 190	 % �� .R��� $��tblServerStatName��$* 5,0,0,0q����� *SectionName+$ TwoWay=�$ PropertyChanged=�+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)����� *SectionTooltip+$ TwoWay=�$ PropertyChanged=�  $$ 70	 % �� .R��� $��btnEdit��#��[ $��60��3 �� *
StatName+���� *$"DataContext.EditExistingStatistics+' �� $( DataGridL����� .���� $, 
Horizontal=�.+��� $3��-/StatTickerFive;component/Images/editIcon.png��$��17��$��20���� $��Calibri)�$��14(�
���   ����� *
EditName++ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��EditStatAvail��$ ��Edit Statistics Properties��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)�- �� $
. true��/ �� *isGridChecked+.a��� $0 btnEdit��1 � {�2 . �� $. false��/ �� *isGridChecked+.a��� $0 btnEdit��1 � {�2 .    $	$ 195	 % �� .R��� $* 5,0,0,0q����� *DisplayName+$ TwoWay=�$ PropertyChanged=�+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��DisplayNameAvail��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)����� *DisplayTooltip+$ TwoWay=�$ PropertyChanged=��� $��1��  NormalButton#��[ $
��Save��$��23��$��75��$��Right=�$��0,5,2,2q����� *SaveStatistics++ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��SaveSettings��$��Save Settings��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$��2��#��[ $
��Skip��$��23��$��75��$
��Left=�$��2,5,0,2q����� *ConfigSkipCmd++ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��SkipSettings��$��Skip Settings��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)��� $	��395��$	��669��$��2��$��1��
�������$��1q����#D6D7D6._��� $ 2,2,0,0��._��� $��3q�$	��658��$	��385��$	 Top=�.@��� $
��Left=����� *Title+.��� $
��auto��$	��380�� �� $��30 ��� $��* �.+��� $��0��$��0��$��4��$	��376��$
��Auto��
�������$��1q����#D6D7D6$��2q�._��  �� $��20 ��� $��30 �.+��� $��0��$��20��$��20��$
��Left=�
������   ImageButton#��[ {��� *IsSavebtnEnable+$ TwoWay=�$ PropertyChanged=����� *PropertySave+.���� $/��)/StatTickerFive;component/Images/save.png��$��18��$��18��   $	 
MouseEnter��.
    �� *IsSavebtnEnabledCommand++ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -	   $��SaveProperties��$��Save Properties��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)���`� 4 `� 	Converter#4 [  .1��� $ txtSectionName��$
 Text��� $ cboxSectionName��$ SelectedItem��� $ txtStatName��$
 Text��� $��1��$��20��$��24��$
��Left=�
������ #��[ ���� *CloseObjectvaluesCommand+.���� $/��)/StatTickerFive;component/Images/Back.png��$��18��$��16��$��2q�+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -
   $��BackSettings��$��Cancel Settings��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��0��$��1��$	��376��$
��Auto��
�������$��1q����#D6D7D6$��2q�._�� $
��Auto�� �� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� $��33 ��� ���� *DataGridRowHeight+ �� $	��125 ��� $��75 ��� $��75 ��� $��* �.+��� $��0��$��0��$��Section Name���� -   $��cboxSectionName��$��1��$��0��$��25��$��66��$��6,0,0,0q�{�. ���� *FilterBorder+���� *
Sections+$ PropertyChanged=�$ TwoWay=�5 �� SelectedItem5 �� *SelectedSection+$ PropertyChanged=�$ TwoWay=�   $	 SelectionChanged��.
    �� *SectionChanged+ �� $ cboxSectionName��$ SelectedItem�1
  +Microsoft.Windows.Controls.WatermarkTextBox
  -   $��txtSectionName��$3 SectionNameWaterMarkText��$��0��$��2��$��0��$	��170��$��25��$��2��$��0,0,0,0q�6 
  	Watermark6 �� *SectionNameWaterMarkText+$ PropertyChanged=�$ TwoWay=����� *SectionNameWaterMarkColor+$ PropertyChanged=�$ TwoWay=����� *StatSectionName+$ PropertyChanged=�$ TwoWay=�.��   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $	 	LostFocus��.
    �� *TextBoxLostFocusCommand+ �� $ txtSectionName��$	 Tag�  $	 TextChanged��.
    �� *TextChangedCommand+.��7 
  WatermarkTemplate7 �� .R��� $��Italic$����� .���� $��0��$��1��$��Display Name��
  -   $��txtDisplayName��$3 DisplayNameWaterMarkText��$��1��$��25��$	��242��$��3,0,0,0q�$��1��$��1��$��3��6 �� *DisplayNameWaterMarkText+$ PropertyChanged=�$ TwoWay=����� *DisplayNameWaterMarkColor+$ PropertyChanged=�$ TwoWay=����� *PropDisplayName+$ PropertyChanged=�$ TwoWay=�.��   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $	 	LostFocus��.
    �� *TextBoxLostFocusCommand+ �� $ txtDisplayName��$	 Tag�  $	 TextChanged��.
    �� *TextChangedCommand+.��7 �� .R��� $��Italic$����� .���� $��0��$��2��$��Statistics Name��
  -   $��txtStatName��$��25��$!3 StatisticsNameWaterMarkText��$
��Auto��$��2��$��1��$��6,0,1,0q�{�.  6 �� *StatisticsNameWaterMarkText+$ PropertyChanged=�$ TwoWay=����� *StatisticsNameWaterMarkColor+$ PropertyChanged=�$ TwoWay=����� *StatisticsName+$ PropertyChanged=�$ TwoWay=����� *StatNameColSpan+.��   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $	 	LostFocus��.
    �� *TextBoxLostFocusCommand+ �� $ txtStatName��$	 Tag�  $	 TextChanged��.
    �� *TextChangedCommand+.��7 �� .R��� $��Italic$����� .���� $��2��$��2��$��3��$��Search��#��[ $��95��$��25��$��0,0,3,0q�y��� *StatNameSearch+���� *StatisticsSearch++ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��SearchStats��$��Search Statistics��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��0��$��3��$��Filter���� -   $��Ftype��$��3��$��23��$	��242��$��3,0,0,0q�{�. $��3��$��1��$��3������ *FilterBorder+���� *FilterSource+$ TwoWay=�$ PropertyChanged=�5 �� *SelectedFilter+$ PropertyChanged=�$ TwoWay=�   $	 SelectionChanged��.
    �� *FilterTypeChangedCommand+ �� $ Ftype��$ SelectedValue�  $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue��� $��0��$��4��$��Format���� -   $��FormType��$��4��$��25��$	��242��$��3,0,0,0q�{�. $��4��$��1��$��3������ *FormatSource+$ PropertyChanged=�$ TwoWay=�5 �� *SelectedFormat+$ PropertyChanged=�$ TwoWay=�   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $	 SelectionChanged��.
    �� *FormatTypeChanged+ �� $ FormType��$ SelectedItem��� $��0��$��5��$��Tooltip value��
  -   $��TTValue��$3 TooltipWaterMarkText��$��5��$��3,0,0,0q�$��25��$	��242��{�. $��5��$��1��$��3��6 �� *TooltipWaterMarkText+$ PropertyChanged=�$ TwoWay=����� *TooltipWaterMarkColor+$ PropertyChanged=�$ TwoWay=����� *TooltipValue+$ PropertyChanged=�$ TwoWay=�.��   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $	 	LostFocus��.
    �� *TextBoxLostFocusCommand+ �� $ TTValue��$	 Tag�  $	 TextChanged��.
    �� *TextChangedCommand+.��7 �� .R��� $��Italic$����� .���� $��0��$��6��$��Statistics color��#   Xceed.Wpf.Toolkit.ColorPicker  $��25��$��6��$	��242��$��1��$��3,0,0,0q�$��6��$��3��8   DisplayColorAndName8 . 9   SelectedColor9 �� *StatisticsColor+   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue��� $��0��$��7��$��Threshold level 1��
  -   $��
txtTvalue1��$3 Threshold1WaterMarkText��$��7��$��2��$��1��$��7��$��25��$��3,0,0,0q�$	��143��6 �� *Threshold1WaterMarkText+$ PropertyChanged=�$ TwoWay=����� *Threshold1WaterMarkColor+$ PropertyChanged=�$ TwoWay=����� *
Threshold1+$ PropertyChanged=�$ TwoWay=�.��   $	 	LostFocus��.
    �� *TextBoxLostFocusCommand+ �� $ 
txtTvalue1��$	 Tag�  $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $	 TextChanged��.
    �� *TextChangedCommand+.��7 �� .R��� $��Italic$����� .��  $��7��$��8��$��3��$��25��$��3,0,3,0q�8 .  $��93��9 �� *ThresholdColor1+   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue��� $��0��$��8��$��Threshold level 2��
  -   $��
txtTValue2��$3 Threshold2WaterMarkText��$��9��$��1��$��8��$��2��$��25��$	��143��$��3,0,0,0q�6 �� *Threshold2WaterMarkText+$ PropertyChanged=�$ TwoWay=����� *Threshold2WaterMarkColor+$ PropertyChanged=�$ TwoWay=����� *
Threshold2+$ PropertyChanged=�$ TwoWay=�.��   $	 	LostFocus��.
    �� *TextBoxLostFocusCommand+ �� $ 
txtTValue2��$	 Tag�  $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�  $	 TextChanged��.
    �� *TextChangedCommand+.��7 �� .R��� $��Italic$����� .��  $��8��$��10��$��3��$��25��$��3,0,3,0q�8 .  $��93��9 �� *ThresholdColor2+   $	 GotFocus��.
    �� *DataGridLostFocusCommand+ �� $ SearchStatnameDataGrid��$ SelectedValue�� -   $��SearchStatnameGrid��$��9��$��4��$	��300��$��3,0,3,0q�.+�  -   $��SearchStatnameDataGrid��$ Column=�$  Disabled=�:   	RowHeight$: 25Q� �#D6D7D6 �#D6D7D6$! Visible=�$��0q�$	��300�� .   .  #��[ 
����������� *SearchStatnameCollection+���� *TitleBackground+#   $	$ 160	 $) StatName��% �� .R�	� $��GridRadiobtn��$��3,0,0,0q�; 	� 	GroupName$; 	StatNames������ *SearchedStatname+$ TwoWay=�$ PropertyChanged=�   $	 Checked��.
    �� *,*DataContext.DataGridRadiobtnCheckedCommand+' �� $( DataGridL� �� *SearchedStatnameTooltip++ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)����� *SearchedStatnameTooltip+$ TwoWay=�$ PropertyChanged=�  $	$ 188	 $) Stat Description��% �� .R��� $* 5,0,0,0q�$��1,0,1,0q����� *SearchedStatDescription+$ TwoWay=�$ PropertyChanged=�+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��DescriptionNameAvail��$��0��$* 	10,5,10,0q�$
��Bold"�$��Calibri)����� *SearchedStatDescTooltip+$ TwoWay=�$ PropertyChanged=�