   M S B A M L   `   `   `  ����S  OPointel.TaskbarNotifier, Version=5.0.1.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier  5   	   ! Pointel.Interactions.Outbound=   7Pointel.Interactions.Outbound.WinForms.CampaignNotifier   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     5   	   6my-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � AllowsTransparency  .  � WindowStyle$
 None=�5   Z   
������  � Title$ CampaignNotifier��5   	   $
��Auto��6"   $	��400��60    � SizeToContent$ Height=�6<    � Topmost .  � 	Resources 5       �� MergedDictionaries �� Source�� 5	   
    5
      �� 5      $V P/Pointel.Interactions.Outbound;component/Resources/StylesResourceDictionary.xaml?�6%   %    5      .��5      �� -   5        $��
MainBorder��5   X   $
��0.89q�6   $��8q�6$   
����������#0070C5._�5   
   �  � RowDefinitions 5      �� 5      $
��Auto �6    60   �� 5      6"   �� 5      $
��Auto �6    60   .+�5      �� $��0��6   $��14(�6!   $��Calibri)�6/   ���#007edf
�������$��Outbound Campaign��6l   6�   �� 5      $��1��6   $��2q�6"   ��6-   �� *BroadCastBackgroundBrush+._�5      � -   5        $��grd_Details��5      
�������$��1,10,1,1q�6=   	 J� Opacity$		 0.9Q�6O   
 � ColumnDefinitions
 5      �� 5      $��70 �6+   68   �� 5      6-   .+�5      �� $��Center=�6   $��Center=�67   $��50��6T   $��50��6_   $B��</Pointel.Interactions.Outbound;component/Images/Outbound.png��6k   6�   �� 5      -   5        $��Details��5   ]   $��1��6!   $��5,0,0,0q�61   $��Center=�6B    �� Orientation$ Vertical=�6l   .+�5      �� $��13(�6E   $��Calibri)�6S   $
��Bold"�6h   $��2q�6z   ��6%   �� *CampaignStatus+6z   � 5        5!      �� 5"   "   62   �� 5#   "   ��60   �� *GRCampaignModeHeight+6Z   
 5%      �� 5&   "   65   �� 5'   "   65   .+�5)      �� $��Name:��6)   $��12(�66   $��Calibri)�6D   $
��Bold"�6Y   $��0��6k   $��0��6x   $��2q�6�   �� 5*      $��12(�6G   $��Calibri)�6U   $��Normal"�6j   $��0��6~   $��1��6�   $��2q�6�   ��6)   �� *CampaignName+6�   �� 5+      $��Delivery Mode:��6)   $��12(�6?   $��Calibri)�6M   $
��Bold"�6b   $��1��6t   $��0��6�   $��2q�6�   �� 5,      $��12(�6G   $��Calibri)�6U   $��Normal"�6j   $��1��6~   $��1��6�   $��2q�6�   ��6)   �� *DeliveryMode+6�   5-      5.      5/      50      �� 51      -   5        $��btnOk��51   3   $��2��6   $��5,5,5,5q�6"   $y�Visible=�6@      NormalButton#��[  6U   $��Right=�6{   $��23��6�   $��80��6�   $��Ok��6�   6�   52      53      54      