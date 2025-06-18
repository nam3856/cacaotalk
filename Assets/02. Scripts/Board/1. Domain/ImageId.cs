using System;

public readonly struct ImageId : IEquatable<ImageId>
{
    public string Value { get; }

    public ImageId(string value)
    {
        Value = value?.Trim();  // 유효성 검사는 하지 않음
    }
    public bool IsEmpty => string.IsNullOrEmpty(Value);

    public override string ToString() => Value;
    public bool Equals(ImageId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}