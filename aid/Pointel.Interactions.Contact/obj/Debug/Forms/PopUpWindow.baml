   M S B A M L   `   `   `  ����   Pointel.Interactions.Contact4    .Pointel.Interactions.Contact.Forms.PopUpWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � AllowsTransparency  .  � WindowStyle$
 None=�5   Z    � WindowStartupLocation$ CenterScreen=�6m    � ShowInTaskbar .   � Topmost . 
������ $
��Auto��5   1   $	��400��6?    � SizeToContent$ Height=�6K    � 	Resources 5         NormalButton �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� CornerRadius J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName :� RoutedEvent �� TargetProperty W� From	 W� To g� Duration g� AutoReverse��%  &	        �� 5   
   " ��6%   .�5      ��  � ��	 5	      �� 5
      "
 ��6&   .R�5      �� $��Border��6!   $ 2��6/   ���#939589
�������$��Calibri)�6r   $��14(�6�   ._�5      �� $��content��6/   #��z��6>   #��z��5   '   ��. # z 5   '   #��z��5   '   5       5      P� 5       �@J�IsMouseOver . . �5   "   �� $ Border��6)    � ��	 �#007edf6S   5      :� 5      $ Button.Click��6+   .[�5   "   �� .��5   &   �� .��5   *   W� $ (Button.Opacity)�6:   $ 1X�6g   $
 0.25X�6p   $ 0:0:0.25D�6z    . 6�   5   '   5   #   5      P� 5       � �� . . �5   "   �� $ Border��6)    � ��	 �#0061ac6S   5      5!      5#      5$      5%      .��5&      � .+�5'   
   �� -   5        $��
MainBorder��5'   F   $
��0.89q�6   $��8q�6(   
������� J� BitmapEffect 5(      F� 5)       F� Opacity$	 0.5Q�6)    F� Softness$	 0.5Q�67    F� ShadowDepth$ 0Q�6F    F� Color$ #003660��6V   ._�5+      �  � RowDefinitions 5,      �� 5-      $��28 �6$   �� 5.      .+�50      �� -   5        $��txtName��50   J   $��0��6   $��Agent Interaction Desktop��6%   $��14(�6Y   $��Stretch=�6g   $��Calibri)�6�   ���#007edf
�������6�   �� 51      $��1��6   ���#656d90$	��0.5q�6=   $��5q�6S   ._�52      �  53      �� 54      $��65��6,   $��* �6;   �� 55      $
��Auto �6,   .+�57      �� -   5        $��lblAlert��57   $   $��0��64   $
��Wrap=�6A   $��Calibri)�6U   $��12(�6j   
���   �$��8q�6�   6�   � 59      $��1��6$   $��Right=�61   $��2q�6M   .+�5:      �� -   5        $��btnOK��5:   &   $��23��63   $��80��6?   $��0,10,2.5,10q�6J   $��Right=�6_   $��Bottom=�6{   $	��Yes��6�   #��[  6�   6�   �� 5;      -   5        $��	btnCancel��5;   &   $��23��67   $��80��6C   $��2.5,10,5,10q�6N   $��Right=�6c   $��Bottom=�6   $��No��6�   #��[  6�   6�   5<      5=      5>      5?      5@      5A      5B      