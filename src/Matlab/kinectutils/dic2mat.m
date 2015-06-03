function [dictmat] = dic2mat(dic)

    keys = dic.Keys;
    keys_enum = keys.GetEnumerator;
    len = dic.Count;
    keymat = cell(len,1);
    cnt = 1;
    stat = 1;
    while stat
       stat = keys_enum.MoveNext;
       if stat
           key = keys_enum.Current;
           if ~isempty(key)
               keymat{cnt} = char(key);
           end
       end
       cnt = cnt + 1;
    end

    values = dic.Values;
    vals_enum = values.GetEnumerator;
    valmat = cell(len, 1);
    cnt = 1;
    stat = 1;
    while stat
       stat = vals_enum.MoveNext;
       if stat
           val = vals_enum.Current;
           if ~isempty(val)
               valmat{cnt} = val;
           end
       end
       cnt = cnt + 1;
    end

    dictmat = [keymat, valmat];
end