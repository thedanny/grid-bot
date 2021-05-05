namespace GridBot.Core.Models
{
	public class Asset
	{
		public Asset(string id, AssetType type)
		{
			ID = id;
			Type = type;
		}

		public string ID { get; }
		public AssetType Type { get; }

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Asset) obj);
		}

		protected bool Equals(Asset other)
		{
			return ID == other.ID;
		}

		public override int GetHashCode()
		{
			return (ID != null ? ID.GetHashCode() : 0);
		}

		public static bool operator ==(Asset left, Asset right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Asset left, Asset right)
		{
			return !Equals(left, right);
		}
	}
}