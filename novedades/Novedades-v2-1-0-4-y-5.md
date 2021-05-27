# gsNotas.Mobile v2.1.0.5

Ver el contenido en mi blog: https://www.elguillemola.com/gsnotas-mobile-v2-1-0-4-y-0-5/


<h1>gsNotas.Mobile multiplataforma<br>(versiones 2.1.0.4 y 2.1.0.5)</h1>

<blockquote class="wp-block-quote"><p><strong>Nota:</strong><br>Esta es la página para la última versión publicada de <strong>gsNotas multiplataforma</strong>.<br>Si estás buscando la <a href="https://www.elguillemola.com/gsnotasnet-android-v2-0-0-33/">versión 2.0.0.33</a> sigue el enlace.&nbsp;</p></blockquote>

<p>En la última actualización de ayer 26 de mayo de 2021 (la terminé de compilar, etc. ayer día 26 pero hoy 27 de mayo es cuando la hago público y pondré <a href="https://github.com/elGuille-info/gsNotas.Mobile" target="_blank" rel="noopener">el código fuente de esta última release en GitHub</a>) ha habido unos cambios con respecto a la última versión, entre ellos que está disponible para Windows UWP (Universal Windows Platform) y estará publicada en la Microsoft Store cuando este sitio tenga el certificado de seguridad, que aún no tiene por aquello del cambio de proveedor de hosting y el latazo de los cambios de registro de los dominios, etc.).</p>

<p>El nombre cambia a gsNotas (gsNotas.Mobile) pero que cada plataforma tendrá el suyo propio: <strong>gsNotas.Android</strong> para Android, <strong>gsNotas.UWP</strong> para la plataforma universal de Windows y <strong>gsNotas.iOS</strong> para el iPhone (en teoría para cualquier dispositivo iOS).</p>

<p>La versión para iOS (iPhone) ya he podido hacerla funcionar en el móvil que tengo: iPhone 7 plus.<br>Aún no puedo crear la aplicación para la App Store de Apple porque necesito un Mac con la última versión de Xcode, y el Mac que tengo actualmente a mi disposición (a través de conexión remota), el de mi nuera Olena, no tiene la última versión del Mac OS por tanto no puedo tener el último Xcode.<br>Aunque me ha comentado mi hijo David que está por actualizarlo, aunque esa última versión del MacOS no es de su agrado... se ve que tiene fallos...&nbsp;</p>

<p><del>La versión para iOS (iPhone, etc.) tendrá que esperar, ya que había problemas entre Microsoft y Apple con el tema de los certificados, etc., y parece ser que ya lo han solucionado, el problema es que para poder crear el paquete de instalación necesito un Mac con la última versión de Xcode y el que tengo disponible es una versión del iOS que no soporta esa última versión de Xcode, así que... a esperar o a ver si un alma caritativa me ofrece un Mac de forma remota con la última versión del iOS.</del></p>

<p>&nbsp;</p>

<h2>Novedades en esta versión (2.1.0.4~2.1.0.5)</h2>

<h3>v2.1.0.5 (27-mayo-2021)</h3>

<p>Aparte de poder compilar y probar la versión para iOS en un iPhone 7 Plus, he añadido código para que los <strong>Placeholder</strong> (el texto mostrado como indicador de qué se espera que escribas) se vean también en modo oscuro (en modo <em>light</em>/claro se veían bien, pero desaparecían en modo <em>dark</em>/oscuro).<br>Y es que yo suelo tener mi móvil (de Android) en modo oscuro, más que nada por el tema del ahorro de batería y normalmente no se veían esos <em>placeholders</em> salvo que cambiara a modo claro (quitando el ahorro extremo de batería).</p>

<p></p>

<h3>Novedades cosméticas</h3>

<p>La mayoría de estas novedades son más bien cosméticas, es decir, un pequeño lavado de cara, pero no en plan grande, si no, <em>de apoco</em> que dirían mis colegas del otro lado del charco; como por ejemplo, dividir todas las opciones que antes se mostraban en la pantalla de inicio en dos pantallas (ahora pondré las capturas).<br>De esta forma, las más usuales estarán en la primera pantalla y el resto en la otra.</p>

<p>Pero también hay novedades en cuanto a la funcionalidad.</p>

<h3>Novedades funcionales</h3>

<p>El otro día (ya no recuerdo qué día) me cambié de compañía telefónica en el móvil, tenía <strong>Digi Mobil</strong> y me cambié a <strong>Amena</strong>. El cambio fue porque yo vivo en el campo y aquí no hay cobertura telefónica, esto es como un agujero negro para las compañías telefónicas, solo tengo acceso por medio del Wi-Fi, pero no porque una compañía telefónica lo ofrezca, no, para eso también es un agujero negro, es porque un colega tiene un repetidor de señal que trae desde el pueblo (Nerja) hasta aquí los montes (Río de la Miel), y hasta ahora toda la comunicación era por medio de Internet. Pero los de Orange ofrecen un servicio llamado Llamadas por Wi-Fi, y como Orange es caro (para mí) me decidí por Amena que tiene una tarifa aceptable. <br>Retomamos la historia, sin tantas <em>historias</em>:<br>Y en ese transcurso del cambio de compañía, perdí la conexión a Internet mientras estaba en el pueblo, como esta utilidad la suelo usar para tomar notas, quise abrirla para consultar algo, y resulta que me decía que no había conexión a Internet, así que, decidí usar las notas locales, pero... ¡No me las mostraba! ¿Por qué? Porque no había conexión a Internet... ¿¿¿???<br>Así que... me dije a mí mismo:<br>A ver Guille... si las notas locales están para usarlas cuando no hay conexión y no se pueden usar cuando no tienes conexión... ¿para qué puñetas sirven?<br>Y me respondí:<br>¡Poh e verdá! (yo es que conmigo mismo hablo así)</p>

<p>Así que... esa es una de las <em>novedades funcionales</em> de la nueva versión:<br>¡Que se puede trabajar de forma local aunque no estés logueado!</p>

<p>Por tanto, y para que quede evidente, en la pantalla de LOGIN he añadido un botón para conectar de forma local: <strong>Acceder sin conexión</strong>.</p>

<p>La segunda novedad importante es que las notas permiten más de 2048 caracteres.<br>Por ahora no está accesible a todo el mundo, lo siento, solo para los que hayan hecho donativos de 25€ o más.<br>Pero no te preocupes, las notas se pueden seguir guardando aunque no guarde más de 2048 caracteres.<br>En realidad, la aplicación usa dos tablas de notas: <strong>Notas</strong> y <strong>NotasMax</strong>.<br>En la primera (<strong>Notas</strong>) usa un campo (Texto) de tipo <strong>nchar(2048)</strong>, lo iba a cambiar a nchar(4000) que es el máximo permitido para nchar, pero para ello tenía que eliminar todos los datos que ya había (recrear la tabla) y... pues como que no era plan.<br>Así que, he creado la segunda tabla (<strong>NotasMax</strong>) en la que el campo Texto es de <strong>nvarchar(Max)</strong>, es decir, con capacidad de 2GB máximo que viene a ser de 1GB de caracteres.<br>He probado el rendimiento tanto en Android como en Windows y va bien.</p>

<p>Como te he dicho antes, esa otra tabla solo está accesible si has donado 25€ o más.<br>Si no es así, no puedes seleccionar cuál de las dos tablas usar y por tanto, usará la tabla normal de 2048 caracteres máximo, que creo que para notas normales es más que suficiente.<br>La tabla que estás usando se muestra en la barra de estatus (abajo). Si usas las notas locales, la capacidad será la que da <strong>SQLite</strong>, que la verdad no sé cuál es. <a href="https://es.quora.com/Qu%C3%A9-volumen-de-datos-puede-manejar-SQLite" target="_blank" rel="noopener">Sigue este enlace</a> si quieres leer sobre el tema (aunque a mí no me queda claro cuántos caracteres se puede tener en un campo de texto).</p>

<p>Otros de los cambios, que serían más bien estéticos, es el coloreado de los textos y demás. Lo he probado con el tema claro y oscuro de mi móvil Android y va bien. En el emulador siempre van bien los dos tipos de temas, pero cuando la uso desde el móvil "de verdad" no siempre se muestran todos los textos.<br>En Windows también va bien con el tema oscuro, aunque en realidad solo afecta a los campos de edición, el resto se muestra igual con los dos tipos de temas.<br><br>Con la versión 2.1.0.5 ya está solucionado lo de los <strong>Placeholder</strong> en Android (en iOS y UWP no era problema).<br><br><s>Aunque en Android el texto de ayuda en los campos sigue sin mostrarse cuando se usa el tema oscuro, es decir lo que se indica en <strong>Placeholder</strong>. Algún día espero poder solucionarlo. :-)</s></p>

<p>La configuración la guardo ahora en un fichero de texto interno (en <strong>Environment.SpecialFolder.LocalApplicationData</strong>) ya que al guardarlo como hasta ahora en <strong>Application.Current.Properties</strong>, no siempre recuperaba esos valores... Lo mismo es por fallo mío, pero... uno de mis dichos es: Si las cosas no siempre funcionan igual, intenta usar lo que siempre debería funcionar.<br>Por tanto, si has estado usando la versión anterior, puede que los valores de la configuración no te los lea cuando abras por primera vez esta nueva versión.</p>

<h2>Cosas diferentes en la app de Android de la de UWP</h2>

<p>Aunque no son funcionales, hay un par de cambios entre las app de Android y la de Windows (UWP), y es que en la de Windows (en escritorio que es como la utilizo) la flechita de ir atrás es muy pequeña, así que... he añadido botones para ir atrás que en la aplicación de Android (u otra que no sea de UWP) no se muestran. Esos botones solo sirven para ir a la página anterior, por tanto, no son realmente funcionales si no, más bien estéticos.</p>

<p>Respecto a esto, el botón de añadir nueva nota, en Android siempre se mostraba (+Nota), pero en UWP se mostraba el menú ese de tres puntitos a la derecha y quedaba como oculto esa posibilidad de agregar una nueva nota, así que... he optado por poner un botón bien visible para que quede claro si se quiere añadir una nueva nota qué es lo que hay que hacer.</p>

<p>Y creo que esto es todo.<br>Ahora te pondré un par de capturas tanto de la app de Android (en dispositivo real) como de UWP en la versión escritorio.</p>

<p>&nbsp;</p>

<h2>Descargas de las aplicaciones</h2>

<p>Comentarte que a día de hoy 27 de mayo de 2021, la aplicación de UWP en Mocrosoft Store aún no está disponible, ya que me la rechazan porque el acceso a la política de privacidad no se muestra de forma correcta. Y esto es porque al acceder a este blog que aún no tiene el certificado de seguridad para usar HTTPS, pues... eso... dicen que nones.<br>Cuando esté disponbible pondré el enlace a la tienda.</p>

<p><strong>La de Android</strong> la puedes descargar desde GitHub.<br>Este es <a rel="noreferrer noopener" href="https://github.com/elGuille-info/gsNotas.Mobile/releases/tag/v2.1.0.5" target="_blank">el enlace para la release v2.1.0.</a><a href="https://github.com/elGuille-info/gsNotas.Mobile/releases/tag/v2.1.0.5" target="_blank" rel="noreferrer noopener">5 en GitHub</a>.</p>

<p></p>

<h2>Donativos con PayPal</h2>

<p>Como podrás ver en las capturas (figuras 3 y 6) he puesto un botón por si quieres <strong><a rel="noreferrer noopener" href="https://www.paypal.com/donate?hosted_button_id=E48GY5YNX8AMS" target="_blank">Hacer un donativo con PayPal</a></strong> que te llevará a la misma página que el enlace anterior.<br>Ese donativo figurará como que es para <strong>gsNotas</strong> y por tanto te servirá para ir acumulando y acceder a las características de los donadores. ;-)<br>Ya sabes que todo esto es gratis y aunque le dedico bastante tiempo no es que quiera cobrar por ese tiempo... pero ya sabes que de algo hay que vivir... pagar la comida, el alquiler de la casa, los impuestos, la conexión a Internet, etc. Así que... si te sientes generoso y quieres (y puedes hacerlo), ahí está el botón de hacer donativos. Imagina que me quieres invitar a un café o a una chela... pues... ¡ya sabes qué hacer! ;-)</p>

<p></p>

<h2>Capturas</h2>

<p><strong>De Android con el Placeholder solucionado en v2.1.0.5.</strong><br>El texto en gris: <em>Escribe aquí tu comentario</em> antes no se veía cuando estaba seleccionado el terma oscuro. Que aunque en la captura no lo parezca, está usando el tema oscuro.</p>

<figure class="wp-block-image size-large is-resized"><img src="https://www.elguillemola.com/img/img2021/gsNotas-Android-164234.jpg" alt="Figura 9" width="650" height="1408"/><figcaption>Figura 9. En Android (tema oscuro aunque no lo parezca) ya se ve el texto de los Placeholders.</figcaption></figure>

<p></p>

<p><strong>De iPhone (iOS)</strong></p>

<figure class="wp-block-image size-large is-resized"><img src="https://www.elguillemola.com/img/img2021/gsNotas-iOS-IMG_5012.PNG" alt="Figura 8" width="650" height="1155"/><figcaption>Figura 8. Captura de la pantalla de inicio en un iPhone 7 Plus.</figcaption></figure>

<p></p>

<p><strong>De Windows (UWP) Escritorio</strong></p>

<figure class="wp-block-image size-large is-resized"><img src="https://www.elguillemola.com/img/img2021/gsNotas-UWP-103554.png" alt="" width="650" height="504"/><figcaption>Figura 1. La página de inicio en Windows (UWP Desktop).</figcaption></figure>

<figure class="wp-block-image size-large is-resized"><img src="https://www.elguillemola.com/img/img2021/gsNotas-UWP-103635.png" alt="" width="650" height="505"/><figcaption>Figura 2. La página de acceso (Login) en Windows UWP.</figcaption></figure>

<figure class="wp-block-image size-large is-resized"><img src="https://www.elguillemola.com/img/img2021/gsNotas-UWP-103740.png" alt="" width="650" height="503"/><figcaption>Figura 3. Las opciones de configuración en UWP. Puedes ver el botón Volver que no está en Android.</figcaption></figure>

<p></p>

<p><strong>De Android (dispositivo Pixel 4a)</strong></p>

<figure class="wp-block-image is-resized size-large"><img src="https://www.elguillemola.com/img/img2021/gsNotas-Android-103411.png" alt="" width="650" height="1408"/><figcaption>Figura 4. La página de inicio en dispositivo Android (Pixel 4a).</figcaption></figure>

<figure class="wp-block-image is-resized size-large"><img src="https://www.elguillemola.com/img/img2021/gsNotas-Android-103421.png" alt="" width="650" height="1408"/><figcaption>Figura 5. La página de Otras opcione en dispositivo Android (Pixel 4a).</figcaption></figure>

<figure class="wp-block-image is-resized size-large"><img src="https://www.elguillemola.com/img/img2021/gsNotas-Android-103435.png" alt="" width="650" height="1408"/><figcaption>Figura 6. La página de configuración en dispositivo Android (Pixel 4a).</figcaption></figure>

<figure class="wp-block-image is-resized size-large"><img src="https://www.elguillemola.com/img/img2021/gsNotas-Android-103457.png" alt="" width="650" height="1409"/><figcaption>Figura 7. La página de acceso (Login) en dispositivo Android (Pixel 4a).</figcaption></figure>

<p></p>

<p>Nos vemos<br>Guillermo</p>
