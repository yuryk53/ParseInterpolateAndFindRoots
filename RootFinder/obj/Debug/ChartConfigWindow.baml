   M S B A M L   `   `   `  ����E  ARootFinder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:RootFinder
RootFinder  5   	   "    RootFinder.ChartConfigWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   *localclr-namespace:RootFinder     5   	   -   5          � Title$  ChartConfigWindow��5   	   $��269.82��6#   $��433.94��63    � 
ResizeMode$ NoResize=�6B    � 	Resources 5      $  RootFinder.RangeValueConverter   RangeValueConverter'  !RootFinder.AccuracyValueConverter  AccuracyValueConverter��%:   &	        &	        5   
   6$     5   
   6'   5	      .��5
      � $��0,0,2,-1q�6   .+�5   
   �� $3��-Here you can configure some chart properties:��6   $��10q�6J   .��5      � .+�5      �� $��	Accuracy:��6   $
��Left=�6,   $��	10,20,0,0q�6G   $	��Top=�6Z   �� 5      $��
X - Range:��6   $
��Left=�6-   $��	10,51,0,0q�6H   $	��Top=�6[   �� 5      $��
Y - Range:��6   $
��Left=�6-   $��	10,82,0,0q�6H   $	��Top=�6[   �� 5      -   5        $��OK��5      $
��Left=�6&   $��309,169,0,0q�6A   $	��Top=�6V   $��75��6n   6y   �� 5      -   5        $��Cancel��5      $
��Left=�6*   $��203,169,0,0q�6E   $	��Top=�6Z   $��75��6r   6}   �� 5      $��
136,20,0,0q�6    �� Orientation$ 
Horizontal=�61   $
��Left=�6J   $	��262��6e   $��32��6q   $	��Top=�6}   .+�5      �� $	��Low��6   $
��Left=�6*   $	��Top=�6E   �� 5      -   5         ��Name$ sldrAcc��5      $
��Left=�6.   $��0,4,0,0q�6I   $	��Top=�6Z   $��97��6r    � Minimum$ 1Q�6}    � Maximum$ 8Q�6�    � LargeChange$ 1Q�6�    � SmallChange$ 1Q�6�   	 � Value$	 4Q�6�   �� 5      $
��High��6   $
��Left=�6+   $��0,4,0,0q�6F   $	��Top=�6W   �� 5      $
��Left=�6�   $��0,4,0,0q�6�   $	��Top=�6�   $��92��6�   ��6   �� *Value+
 �� 	Converter#
 [  �� ElementName$ sldrAcc�� �� IsAsync . 6�   5      �� 5      $��
136,55,0,0q�6   $ 
Horizontal=�61   $
��Left=�6J   $	��228��6e   $��28��6q   $	��Top=�6}   .+�5      �� $��From:��6   $
��Left=�6,   $	��Top=�6G   $��40��6_   �� 5      -   5         ��Name$ txtXFrom��5      $
��Left=�60   $��23��6K   $��0,3,0,0q�6W    �� TextWrapping$
 Wrap=�6h   $	��Top=�6�   $��60��6�   ��6|   �� *Text+#
 [  $ txtXTo��6�   �� 5      $	��To:��6   $
��Left=�6*   $	��Top=�6E   $��40��6]   $��Right=�6h   �� 5      -   5        $ txtXTo��5      $
��Left=�6.   $��23��6I   $��0,3,0,0q�6U   $
 Wrap=�6f   $	��Top=�6�   $��60��6�   ��6z   �� *Text+#
 [  $ txtXFrom��6�   5      �� 5      -   5         ��Name$ stackYRange��5      $��
136,81,0,0q�62   $ 
Horizontal=�6F   $
��Left=�6_   $	��228��6z   $��28��6�   $	��Top=�6�   .+�5      �� $��From:��6   $
��Left=�6,   $	��Top=�6G   $��40��6_   �� 5       -   5        $ txtYFrom��5       $
��Left=�60   $��23��6K   $��0,3,0,0q�6W   $
 Wrap=�6h   $	��Top=�6�   $��60��6�   ��6|   �� *Text+$ txtYTo��#
 [  6�   �� 5!      $	��To:��6   $
��Left=�6*   $	��Top=�6E   $��40��6]   $��Right=�6h   �� 5"      -	   5        $ txtYTo��5"      $
��Left=�6.   $��23��6I   $��0,3,0,0q�6U   $
 Wrap=�6f   $	��Top=�6�   $��60��6�   ��6z   �� *Text+$ txtYFrom��#
 [  6�   5$      �� 5%      -
   5         ��Name$ 
chkBindYFx��5%      $ ��Bind Y-Range to F(x) range��6/   $
��Left=�6T   $��
10,124,0,0q�6o   $	��Top=�6�   6�   �� 5&      -   5        $ 
chkBindYGx��5&      $ ��Bind Y-Range to G(x) range��6/   $
��Left=�6T   $��
10,145,0,0q�6o   $	��Top=�6�   6�   5'      5(      5*      5+      