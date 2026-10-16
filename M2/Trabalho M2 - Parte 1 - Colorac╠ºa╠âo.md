**Trabalho M2 \- Parte 1**

A parte 1 do trabalho da M2 consiste na implementação do método de força bruta e na implementação de três heurísticas para o cálculo de coloração:

* O método de **força bruta** deverá testar todas as possibilidades de combinação de cores para os vértices, começando com todas as combinações com 2 cores, depois com 3 e assim por diante até encontrar uma combinação que seja viável para a coloração (sem vizinhos com a mesma cor).


* As heurísticas de coloração implementadas devem ser **Welsh Powell** e **DSATUR**, conforme os pseudocódigos dos slides. Também deve ser implementada uma **heurística sem critério de ordem de vértices**, somente escolhendo em uma cor para cada vértice que não esteja em seus vizinhos em ordem arbitrária.


* A saída das soluções deve ser o tempo de execução e o número de cores utilizadas. Para os grafos pequenos (menos de 10 vértices) também deverá ser exibida uma lista com os vértices e as cores correspondentes.

	Deve ser implementado um **contador de tempo**, para computar a duração de execução dos algoritmos de coloração implementados.

	Os grafos virão de arquivos de texto como os utilizados no último trabalho, podendo possuir milhares de vértices e arestas.  
	Todos os grafos serão não ponderados e não direcionados.

Importante:

* A base do trabalho anterior será utilizada neste;  
* Podem manter o grupo do trabalho anterior;