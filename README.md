# gsNotasNET.Android
 Aplicación para tomar notas y usar en dispositivos Android.

> **NOTA sobre los proyectos para iOS y UWP**
>
> He incluido esos dos tipos de proyectos, pero no he sido capaz de hacerlos funcionar ni usando el Visual Studio para Windows.
>
> El proyecto de iOS tampoco lo he podido ejecutr (sin errores) con el Visual Studio para Mac (en un Mac que me han dado acceso remotamente).
>
> El proyecto de UWP no ha funcionado con el Visual Studio para Windows (en el Mac parece ser que no se puede usar), da error, pero no llega ni a ejecutar el código de la aplicación, y no soy capaz de saber qué _falta_ por configurar... :-(
>
> El de UWP (universal Windows Platform) solo se puede usar en el VS para Windows.
>


## Visto lo visto, seguramente crearé otro proyecto que solo acceda a la base de datos local

De esa forma comprobaré si funciona en los tres dispositivos.

Y espero poder usar alguna forma de guardar _en la  nube_ los datos locales.

Pero eso, seguramente será el año que viene ;-) (ahora es 31 de diciembre de 2020 17:12 GMT+1)


<br>
<br>

# Las cosas de la versión 2.0.0.*

## Ayuda sobre la versión 2.0.0.33 del 30 de diciembre de 2020
Mira el contenido en [este enlace a la Ayuda v2.0.0.33](https://github.com/elGuille-info/gsNotasNET.Android/blob/master/Ayuda-v2.0.0.33.md)

El paquete de instalación te lo puedes descargar de [la Release gsNotasNET.Android v2.0.0.33](https://github.com/elGuille-info/gsNotasNET.Android/releases/tag/v2.0.0.33)

<br>
---
<br>
Mira el contenido en [este enlace a la Ayuda v2.0.0.32](https://github.com/elGuille-info/gsNotasNET.Android/blob/master/Ayuda-v2.0.0.32.md)

El paquete de instalación te lo puedes descargar de [la Release gsNotasNET.Android v2.0.0.32](https://github.com/elGuille-info/gsNotasNET.Android/releases/tag/v2.0.0.32)

<br>


Mira el contenido en [este enlace a la Ayuda v2.0.0.31](https://github.com/elGuille-info/gsNotasNET.Android/blob/master/Ayuda%20v2.0.0.31.md)

<br>
<br>
Ahora se accede a una base de datos de SQL Server.

## gsNotasNET.Android v2.0.0.28
Ya está operativa la base de datos (he eliminado todas las notas de prueba).

Puedes registrarte directamente en la aplicación o mandar un mensaje a **coreos.elguille.info@gmail.com** indicando tu nombre completo, cuenta de gmail y el password y te añadiré...

En las releases está el paquete de instalación [**gsNotasNET.Android v2.0.0.28**](https://github.com/elGuille-info/gsNotasNET.Android/releases/tag/v2.0.0.28).

<br>
<br>

## Ya está el instalador para la versión 2.0.0.25 del 25-dic-2020
<br>
Se supone que ya funciona todo... tendré que hacer algunas comprobaciones finales y lo daré por buena esta versión (y las posteriores).<br>
Las notas añadidas hasta que de el pistoletazo de salida no serán permanentes, ya que he hecho muchas de prueba y las voy a borrar antes de poner la aplicación como **publicada** y utilizable, te lo digo por si añades notas y desaparecen, que sepas el porqué.<br>
De todas formas, antes de eliminarlas, comprobaré que no haya notas distintas a las mías.<br>
<br>
<br>

> **NOTA:** <br>
> Debido a que se usa una base de datos de SQL Server y no es plan de compartir las credenciales de acceso.<br>
> Esas credenciales las he guardado en un fichero (**encrypted-string.txt**) que no está aquí publicado.<br>
> Por tanto, si quiers usar este código para crear tu propia versión de la aplicación tendrás que crear ese fichero en el que debes incluir el nombre del usuario y el password, cada dato en una línea.<br>
> También hay un fichero con mis datos de usuario y password (para no tener que ponerlo cada vez que probaba la aplicación).<br>
> Ese fichero se llama **encrypted-string-guille.txt** y tiene el mismo formato que el anterior.<br>
> Ambos ficheros se acceden desde **MainActivity.cs** que está en la aplicación de Android.<br>
> Ahí verás el código de acceso, por si quieres crear el tuyo propio.<br>
> También he quitado las credenciales de acceso a Google Drive.<br>
<br>



# Las cosas de la versión 1.0.0.*

> **Nota:** <br>
> La última versión que usa SQL Lite es la v1.0.0.18 y la puedes descargar de las **releases**<br>
<br>

Las notas se guardan en una base de datos de SQLLite.

Por ahora no funciona la copia de notas en google Docs.

Unas capturas del programa funcionando en mi móvil Google Pixel 4a: 

> Actualizadas las capturas a la versión 1.0.0.17

![alt text](http://www.elguillemola.com/img/img2020/gsNotasNET.Android-04.png "Captura de la aplicación en funcionamiento")

![alt text](http://www.elguillemola.com/img/img2020/gsNotasNET.Android-05.png "Figura 2.")

![alt text](http://www.elguillemola.com/img/img2020/gsNotasNET.Android-02.png "Figura 3.")
