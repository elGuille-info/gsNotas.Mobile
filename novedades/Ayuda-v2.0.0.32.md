# gsNotasNET.Android v2.0.0.32
Utilidad para dispositivos móviles Android para tomar notas y guardarlas localmente o en una base de datos externa.

En esta versión (**v2.0.0.32**) la utilidad/aplicación hace lo siguiente:

Permite crear notas y marcarlas con estos atributos/propiedades:

* Indicar a qué **grupo** (tag/etiqueta) pertenece.

* Se pueden crear tantos grupos como se deseen.

* El programa recordará el último nombre de grupo utilizado al guardar una nota.

* Se pueden ver los grupos que hay creados, mostrando la información de las notas que contiene.

* Al mostrar los grupos se muestra la información de las notas que contiene.

* Al seleccionar un grupo (clic o tap) te muestra la información de tallada de las notas que contiene, así como la lista de esas notas pertenecientes a ese grupo. Desde esa lista puedes editar las notas.


Atributos /Propiedades de cada nota:

* **Favorita**, al mostrar las notas (de la base de datos externa) las notas favoritas se muestran al principio.

* **Notificar**, las notas marcadas para notificar se pueden usar para que al hacer login se muestren automáticamente, ahí puedes anotar los recordatorios que necesites ver cada vez que inicias sesión.

* **Archivada**, esto hará que la nota no se muestre al mostrar las notas activas.

* **Eliminada**, es otra forma de ocultar las notas, ya que **nunca se eliminan** físicamente de la base de datos (tanto local como externa).

* **Sincronizada**, esta propiedad indica si la nota está sincronizada entre las bases local y externa. Las notas siempre se sincronizan entre las dos bases de datos.


Para mostrar las notas hay varios apartados en el menú o pantalla principal, pudiendo mostrar las notas según el atributo o propiedad seleccionado.

* En la lista de notas se muestra la siguiente información:

* El título de la nota que consiste en los primeros 50 caracteres o si hay cambios de líneas en la nota, la primera línea.

* El nombre del grupo, la fecha de modificación y una letra cn el nombre del atributo y si está marcado (True) o no lo está (False).

* Las abreviaturas de los atributos son:

   **F**avorita, **N**otificar, **A**rchivada y **E**liminada.

* En una nueva actualización añadiré **S**incronizada, aunque _se supone_ que siempre deben estar sincronizadas.

* Seguramente también añadiré una página (opción) para mostrar las notas sincronizadas y poder asegurarte que si no está sincronizada, lo haga.


En la **configuración** puedes indicar:

* Recordar el usuario con el que se ha hecho Login.
* Recordar el password usado.
* Iniciar la aplicación con el último usuario.
* Mantener las notas sincronizadas (siempre está activada esta opción).
* Mostrar las notas a **Notificar** al iniciar el programa (o hacer Login).
* Usar las notas locales (cuando se activa) o usar las notas de la base externa.


En la información del **Perfil** te muestra los datos de la cuenta con la que has hecho Login.

+ Ahí puedes cambiar la cuenta de correo y el password.

+ Si cambias la cuenta de correo, se enviarán 2 mensajes, uno a cada cuenta pidiendo confirmación.
+ Te muestra la inforamción de la fecha de alta, último acceso, cuántas notas de la base externa puedes escribir:
+ Para los primeros 90 usuarios que se registren tienen una cuota de 1.000 notas, a partir del usuario 100 (los otros 10 los tengo reservados para mí) tendrán 100 notas como máximo de forma totalmente gratuita.
+ En esa cantidad, se cuentan **todas las notas**, estén o no eliminadas (ya que nunca se eliminan las notas).
+ El importe por cada 1.000 notas será de una **_donación_** anual de 12$ USD (unos 10€). 
+ En esa cuota **no se cuentan las notas ofrecidas gratuitamente**.
+ El importe indicado en **Pagos** será el importe que hayas ido pagando.
+ Ya te digo que solamente debes pagar si quieres más cantidad de notas. **La aplicación es totalmente gratuita**. Salvo que prefieras hacer un _donativo voluntario_.
+ El donativo lo puedes hacer mediante este enlace: [Donativo para elGuille](https://www.paypal.com/donate?token=WBZOeXqlea5enl-OaI8HmUvFhUla47934oYB8N9VGDUHSlKViVP7BDVIEP_cuttlmrwvkPnMMa_AQqxo).
+ Todos los donativos serán siempre bienvenidos :-) 


Puedes realizar búsquedas en las notas (tanto en la base local como en la externa).

* La búsqueda se realiza sin tener en cuenta el _case_ (no diferencia entre mayúsculas y minúsculas).
* Por ahora solo hace la búsqueda en el **Texto** de las notas.
* En una nueva actualización incluiré que se pueda hacer tanbién (o solo) en los nombres de los grupos).
* En el resultado de la búsqueda puedes pulsar (hacer clic o tap) en la nota para editarla.

* Aunque estés usando las notas de la base de datos remota (de SQL Server) puedes mostrar las notas de la base de datos loca, en este caso las notas no son editables, solo se muestran en la lista y haciendo tap o clic en ellas no se muestran.
⋅⋅⋅ Si quieres editar una nota local, debes cambiarlo en la **configuración** seleccionando la opción **Usar las notas locales**.


En la pantalla principal además te encontrarás con estas opciones:

* **Validar Email**. Esto tendrás que usarlo cuando te registres con un nuevo usuario. O al crear un nuevo usuario.
* **Sincronizar**. Esta opción te sirve para comprobar si las notas están sincronizadas (deberían estarlo), pero en caso de que no lo estuvieran, puedes sincronizar las locales al romoto o al revés.
* **Comentarios**. Por si quieres enviarme alguna sugerencia, bug o cualquier cosa que me quieras decir. Esta opción te permite indicar un comentario y al pulsar en el botón de **ENVIAR...** abrirá una cuenta de email que tengas configurada en tu dispositivo para poder enviar el mensaje. El texto que hayas escritor aparecerá en el cuerpo del mensaje.
* **Acerca de** Abre una ventana con un poco de ayuda sobre la aplicación. Seguramente añadiré un enlace a esta página descriptiva, ya que tanto texto no se puede poner en ese tipo de páginas... o al menos no se vería bien.
*  **Cambiar de usuario**. Desde ahí puedes hacer login, si no lo has hecho ya, o bien poder **registrar un nuevo usuario**.


En todas (o casi todas) existe un botón **POLÍTICA DE PRIVACIDAD** que al pulsarlo te muestra (en el navegador) la política de privacidad, es decir, qué datos personales se recogen.

> Ahora muestra el contenido de elguillemola.com: [Política de privacidad en elguillemola.com](https://www.elguillemola.com/politica-de-privacidad/) pero seguramente lo cambiaré a una que sea específica de esta aplicación.
>
> De todas formas, comentarte que con los datos que facilitas o las notas que escribe yo no hago nada, ni cambairé nada, salvo que tú me lo pidas expresamente, bien porque no tengas acceso o cualquier otra circunstancia que te impida acceder a las notas o tu cuenta de usuario.

Bueno y creo que esto es todo... 

He escrito tanto con idea de crear una página en elguillemola.com y así poder usarla como ayuda de la aplicación.

También crearé un video explicativo con las cosas que debes saber sobre la aplicación.


Si has llegado leyendo hasta aquí... ¡muchas gracias! :-)

Y como estamos en las fechas que estamos (30 de diciembre de 2020) te deseo ¡FELICES FIESTAS! 

Guillermo
