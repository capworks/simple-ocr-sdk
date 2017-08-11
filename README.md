# simple-ocr-sdk

## About
Simple OCR SDK is intended to make it easy to integrate OCR usage into you application without any prior knowlegde about OCR, image preparations or the OCR service providers special quirks.

Each OCR provider is a bit different, and like to sort their outputs in their own unique way. When you are past integrating to the API, it's always a hurdle learning the new model and parsing it into something you can use for you futher processing og just plain text. This make the efford of testing an OCR provider to match your needs very time comsuming.

Hopefully this SDK can help you skip the boring stuff and get straight to developing your application.

### How does it work?
The SDK handles the EXIF orientation, and compression if nessesary.
The API result will be transformed into a metadata result model with relative coordinates, that will make it easy for you to highlight your findings in your UI.

### Limitations
The current SDK support Png, jpeg and jpg.

The vision API does not support pdf's. Pdf's need to be converted into images first.
Some PDF's has readable content and can be processed more accurate this way, I recommend using PdfBox. Next expansion to this SDK will be a PDF module and a Azure Vision API.

## Example
There is a demo app demonstrating how to get a OCR result using Google's vision API.
https://github.com/DineroRegnskab/simple-ocr-sdk/tree/master/SimpleOcrSDK/Demo

## Usage

How to init the engine.
```cs
var apiKey = "[Your-google-vision-api-key]";
var applicationName = "[Name-of-of-your-application-for-your-own-tracking-in-google]";

var ocr = GoogleOcrEngine.Build(new GoogleOcrConfigurations(apiKey, applicationName));
```

How to OCR a file from a path.
```cs
string fullPathToImage = "C:\Ocr-images\my-test-img.png";
ImageFormatEnum imageFormat = ImageFormatEnum.Png; //Depends on your image

GoogleOcrResult imageContent = await googleOcr.OcrImage(filePath, imageFormat);
string textInFile = imageContent.TextFound()? imageContent.FormattedResult.GetPlainTextWithLineBreaks() : "";
```

How to OCR a file from Stream.
```cs
Stream myImageStream; //to be assigned by you
ImageFormatEnum imageFormat = ImageFormatEnum.Jpeg; //Depends on your image

GoogleOcrResult imageContent = await googleOcr.OcrImage(myImageStream, imageFormat);
string textInFile = imageContent.TextFound()? imageContent.FormattedResult.GetPlainTextWithLineBreaks() : "";
```


## Result model explained

```cs
public class GoogleOcrResult
{
    public ImageContent FormattedResult { get; }
    public RawGoogleOcrResult RawResult { get; }
}
```

### RawResult
As the name says ```RawResult``` is the raw Google Vision API result. It contains all the result data received from the API.

### FormattedResult
The result is formatted into Lines, Sentences and Words. Words and sentences contains relative coordinates.

```cs
public class ImageContent
{
    /// <summary>
    /// Height of the image (viewing orientation taken into account)
    /// </summary>
    public decimal Height { get; protected set; }

    /// <summary>
    /// Width of the image (viewing orientation taken into account)
    /// </summary>
    public decimal Width { get; protected set; }

    /// <summary>
    /// All sentences found on the image
    /// </summary>
    public List<Sentence> Sentences { get; }

    /// <summary>
    /// All words found on the image.
    /// </summary>
    public List<Word> Words { get;  }

    /// <summary>
    /// All lines found on the image. 
    /// Dictionary with zero index line numbers as key, and list of sentences as value.
    /// </summary>
    public Dictionary<int, List<Sentence>> Lines { get; set; }
```

A Line consist of a number of one or more senteces and a sentece consist of one or more words.

Example of Words:

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/receipt%20explained%20words.jpg "Words")


Example of Senteces:

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/receipt%20explained%20sentences.jpg "Sentences")


Example of Lines:

![alt text](https://github.com/DineroRegnskab/simple-ocr-sdk/blob/master/images/receipt%20explained%20lines.jpg "Lines")




## License

MIT © [dineroregnskab](mailto:info@dinero.dk)
