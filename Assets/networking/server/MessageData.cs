
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class MessageData {

	public string stringData = "top lel";
	public float  mousex = 0;
	public float  mousey = 0;
	public int    type = 0;

	public static MessageData FromByteArray(byte[] input)
	{
		// Create a memory stream, and serialize.
		MemoryStream stream = new MemoryStream(input);
		// Create a binary formatter.
		BinaryFormatter formatter = new BinaryFormatter();

		MessageData data = new MessageData();
		data.stringData = (string)formatter.Deserialize(stream);

		return data;
	}

	public static byte[] ToByteArray (MessageData msg)
	{
		// Create a memory stream, and serialize.
		MemoryStream stream = new MemoryStream();
		// Create a binary formatter.
		BinaryFormatter formatter = new BinaryFormatter();

		// Serialize.
		formatter.Serialize(stream, msg.stringData);

		// Now return the array.
		return stream.ToArray();
	}
}